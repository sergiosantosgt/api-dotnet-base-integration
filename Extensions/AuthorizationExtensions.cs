namespace BaseIntegrationApi.Extensions;

/// <summary>
/// Extensões para configuração de autorização
/// Define policies de acesso baseadas em roles
/// </summary>
public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Policy para exigir role Backend.Access
            options.AddPolicy("RequireBackendAccess", policy =>
                policy.RequireRole("Backend.Access"));

            // Você pode adicionar mais policies conforme necessário
            // Ex: options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
        });

        return services;
    }
}
