using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HikingTrails.ApiGateway.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddJwtBearer(this IServiceCollection services)
    {
        string? secretKey = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET_KEY");
        string? issuer = Environment.GetEnvironmentVariable("ISSUER");
        string? audience = Environment.GetEnvironmentVariable("AUDIENCE");
        
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            throw new Exception("Access token not found");
        
        services
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
    }

    public static string AddAllowSpecificOrigins(this IServiceCollection services)
    {
        string? allowedOriginsStr = Environment.GetEnvironmentVariable("ALLOW_SPECIFIC_ORIGINS");
        
        if (string.IsNullOrEmpty(allowedOriginsStr))
            throw new Exception("Allowed origins are not configured");
        
        string[] allowedOrigins = allowedOriginsStr.Split(',', 
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        string policyName = "_myAllowSpecificOrigins";
        
        services.AddCors(options =>
        {
            options.AddPolicy(name: policyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        return policyName;
    }
}