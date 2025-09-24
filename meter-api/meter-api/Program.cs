using meter_api.Datatypes;
using meter_api.Hubs;
using meter_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// AppSettings
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

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
    });

// Authorization
builder.Services.AddAuthorization();

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
builder.Services.AddSignalR();

// Services
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<MeterHub>("/hub/meters");

app.Run();