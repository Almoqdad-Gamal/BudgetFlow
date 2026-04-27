using System.Text;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.Expenses.Services;
using BudgetFlow.Infrastructure.Jobs;
using BudgetFlow.Infrastructure.Persistence;
using BudgetFlow.Infrastructure.Services;
using BudgetFlow.Infrastructure.Settings;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BudgetFlow.Infrastructure.Extentions
{
    public static class ServiceCollectionExtentinos
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database 
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ApplicationDbContext>());

            // Jwt Settings
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            // Jwt Service
            services.AddScoped<IJwtService, JwtService>();

            // Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Currency Settings
            services.Configure<CurrencySettings>(
                configuration.GetSection("CurrencySettings")
            );

            // Currency Service
            services.AddHttpClient<ICurrencyService, CurrencyService>();

            // Audit Service
            services.AddScoped<IAuditService, AuditService>();

            // Email Settings
            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));

            // Email Service
            services.AddScoped<IEmailService, EmailService>();

            // PDF Service
            services.AddScoped<IPdfReportService, PdfReportService>();

            // Hangfire
            services.AddHangfire(config =>
                config.UseSqlServerStorage(
                    configuration.GetConnectionString("HangfireConnection")));

            services.AddHangfireServer();

            // Monthly Report Job
            services.AddScoped<MonthlyReportJob>();

            // Budget alert
            services.AddScoped<BudgetAlertService>();




            return services;
        }
    }
}