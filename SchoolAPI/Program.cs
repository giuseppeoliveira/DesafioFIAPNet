using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using SchoolAPI;
using SchoolAPI.Application;
using SchoolAPI.Domain.Entities;
using System.Text;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var connectionString = Environment.GetEnvironmentVariable("SchoolAPI_DB_ConnectionString") ?? throw new Exception("Vari�vel de ambiente n�o configurada!");

builder.Services.AddDbContext<SchoolAPI.Infrastructure.SchoolAPIContext>(options =>
    options.UseNpgsql(connectionString)
);

DependencyInjection.CustomOpenApi(builder.Services);
DependencyInjection.CustomCors(builder.Services);
DependencyInjection.AddServices(builder.Services);

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var rawKey = jwtSettings["Key"] ?? throw new Exception("Chave JWT n�o configurada!");
var key = Encoding.UTF8.GetBytes(rawKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "SchoolAPI",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseDefaultFiles(); // Enables serving index.html as the default file
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "www")),
    RequestPath = ""
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
