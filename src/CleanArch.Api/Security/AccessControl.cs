using CleanArch.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CleanArch.Api.Security;

public static class AccessControl
{
    public static IServiceCollection AddSecurityTokenAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var securityTokenValidator = services.BuildServiceProvider().GetRequiredService<ISecurityTokenValidatorService>();
            options.TokenValidationParameters = securityTokenValidator.GetAccessTokenValidationParameters();
            options.MapInboundClaims = false;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access-token"];
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddSecurityTokenAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(Policies.AllowAnonymous, policy => policy.RequireAssertion(_ => true));
            options.AddPolicy(Policies.DeveloperPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("role", "Developer");
            });
        });

        return services;
    }
}
