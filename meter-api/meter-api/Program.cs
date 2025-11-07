using System.Text;
using DotNetEnv;
using meter_api.Attributes;
using meter_api.Datatypes;
using meter_api.Hubs;
using meter_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using meter_api.Datatypes.Database;

var builder = WebApplication.CreateBuilder(args);


Env.Load(); 
builder.Configuration.AddEnvironmentVariables();

static string Require(string? value, string keyName)
    => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"Missing required environment variable: {keyName}")
        : value;

var jwtOptions = new JwtOptions
{
    Secret = Require(Environment.GetEnvironmentVariable("JWT__SECRET"), "JWT__SECRET"),
    Issuer = Require(Environment.GetEnvironmentVariable("JWT__ISSUER"), "JWT__ISSUER"),
    Audience = Require(Environment.GetEnvironmentVariable("JWT__AUDIENCE"), "JWT__AUDIENCE"),
    Expiry = int.TryParse(Environment.GetEnvironmentVariable("JWT__EXPIRY"), out var mins) ? mins : 60
};

var connectionUrl = Require(Environment.GetEnvironmentVariable("DATABASE__CONNECTIONURL"), "DATABASE__CONNECTIONURL");
if (!connectionUrl.EndsWith("/"))
    connectionUrl += "/";

var databaseOptions = new DatabaseOptions
{
    ConnectionUrl = connectionUrl
};

builder.Services.AddSingleton<IOptions<JwtOptions>>(_ => Options.Create(jwtOptions));
builder.Services.AddSingleton<IOptions<DatabaseOptions>>(_ => Options.Create(databaseOptions));

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hub/clients")
                     || path.StartsWithSegments("/hub/agents")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();

                var authHeader = context.Request.Headers.Authorization.ToString();
                var rawJwt = !string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ")
                    ? authHeader["Bearer ".Length..]
                    : (context.SecurityToken as Microsoft.IdentityModel.JsonWebTokens.JsonWebToken)?.EncodedToken
                      ?? (context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken)?.RawData;

                if (string.IsNullOrWhiteSpace(rawJwt) || !jwtService.IsValidJwt(rawJwt))
                {
                    context.Fail("JWT failed validation.");
                }

                return Task.CompletedTask;
            }
        };
    });

// CORS
builder.Services.AddCors(o =>
{
    o.AddPolicy("open", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

// Authorization
builder.Services.AddAuthorization();

// HttpClient
builder.Services.AddHttpClient<DatabaseHttpClient>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new() { Title = "Smart Meter API", Version = "v1" });
    s.AddSignalRSwaggerGen();
});

// SignalR
builder.Services.AddSignalR(options =>
{
    options.AddFilter<AuthorisationHubFilter>();
});

// Services
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<IBillingService, BillingService>();
builder.Services.AddSingleton<IBillingRateService, BillingRateService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IAgentTokenService, AgentTokenService>();
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMeterAgentService, MeterAgentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<ISnapshotService, SnapshotService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("open");
app.MapControllers();

app.MapHub<ClientHub>("/hub/clients").RequireAuthorization();
app.MapHub<AgentHub>("/hub/agents").RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    await dbService.InitialiseDatabase();
}

await app.RunAsync();
