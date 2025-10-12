using meter_api.Attributes;
using meter_api.Datatypes;
using meter_api.Hubs;
using meter_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// AppSettings
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

// Jwt
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing");
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
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/meters"))
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
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAgentTokenService, AgentTokenService>();
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

app.MapHub<MeterHub>("/hub/meters").RequireAuthorization();

app.Run();