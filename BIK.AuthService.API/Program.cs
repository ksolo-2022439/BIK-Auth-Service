using BIK.AuthService.Application.Interfaces;
using BIK.AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;

/// <summary>
/// Punto de entrada del microservicio de Autenticación de BIK.
/// Configura la inyección de dependencias de MongoDB, el esquema de autenticación JWT Bearer, CORS y Swagger.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SEC-009: Restringir CORS a orígenes conocidos
var allowedOriginsSetting = builder.Configuration["AllowedOrigins"];
var allowedOrigins = !string.IsNullOrEmpty(allowedOriginsSetting)
    ? allowedOriginsSetting.Split(',', StringSplitOptions.RemoveEmptyEntries)
    : new[]
    {
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:5000",
        "http://bik-client-user:5173",
        "http://bik-client-admin:5174",
        "http://bik-server-user:5000"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBIKClients", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .SetIsOriginAllowed(origin =>
              {
                  var host = new Uri(origin).Host;
                  return host.Equals("localhost") || 
                         host.Equals("127.0.0.1") || 
                         host.EndsWith(".vercel.app") ||
                         allowedOrigins.Contains(origin);
              })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var mongoConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"] ?? "mongodb://127.0.0.1:27017";
var mongoDbName = builder.Configuration["MongoDbSettings:DatabaseName"] ?? "bik_core_db";

builder.Services.AddSingleton<IAuthRepository>(sp => new AuthRepository(mongoConnectionString, mongoDbName));

var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("Jwt Secret is missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "BIK-AuthService",
            ValidAudience = "BIK-Clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBIKClients");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();