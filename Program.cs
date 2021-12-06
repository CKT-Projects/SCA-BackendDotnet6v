using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using scabackend.Controllers;
using scabackend.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:issuer"],
        ValidAudience = builder.Configuration["AppSettings:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:secret_key"]))
    };
});
builder.Services.AddAuthorization();

// Add cors domains
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins(
            "https://www.scrapcatapp.com",
            "https://test.scrapcatapp.com",
            "https://staging.scrapcatapp.com",
            "http://localhost"
        );
    });
});
IConfiguration configuration = builder.Configuration;
// Initialize configs on AppSettings.Json
builder.Services.Configure<scabackend.Settings.AppSettings>
    (configuration.GetSection("AppSettings"));

builder.Services.Configure<scabackend.Settings.MySQLSettings>
    (configuration.GetSection("MySQLSettings"));

var app = builder.Build();
app.UseSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.UseSwaggerUI();
app.Run();
