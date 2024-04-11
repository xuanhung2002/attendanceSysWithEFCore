using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Services.IServices;
using Services.Services;
using Databases.Models;

namespace Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      ValidateLifetime = false,
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(configuration["JwtSecretKey"]!))
                  };
              });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole(Roles.Admin.ToString()));
                options.AddPolicy("RequireManagerRole", policy =>
                    policy.RequireRole(Roles.Manager.ToString()));
                options.AddPolicy("RequireGeneralEmployeeRole", policy =>
                    policy.RequireRole(Roles.GeneralEmployee.ToString()));
                options.AddPolicy("RequireAdminRoleOrManagerRole", policy =>
                    policy.RequireRole(Roles.Admin.ToString(), Roles.Manager.ToString()));
            });
        }
      
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAttendanceRecordService, AttendanceRecordService>();
        }
    }
}
