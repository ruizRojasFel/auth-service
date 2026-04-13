using Serilog;
using System.Text;
using Auth.API.Application.Common.Interfaces;
using Auth.API.Domain.Interfaces;
using Auth.API.Infrastructure.Identity;
using Auth.API.Infrastructure.Persistence;
using Auth.API.Infrastructure.Repositories;
using Auth.API.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ───────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── DbContext ─────────────────────────────────────────────────
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Unit of Work ──────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AuthDbContext>());

// ── Repositories ──────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// ── Identity Services ─────────────────────────────────────────
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// ── MediatR ───────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ── FluentValidation ──────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ── JWT Authentication ────────────────────────────────────────
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtConfig["Issuer"],
            ValidAudience            = jwtConfig["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Secret"]!))
        };
    });

builder.Services.AddAuthorization();

// ── Serilog ───────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

// ─────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();