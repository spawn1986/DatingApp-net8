﻿using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>();


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKeyy not found");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

        return services;
    }
}
