using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BaseIntegrationApi.Extensions;

/// <summary>
/// Extensões para configuração de autenticação JWT
/// Suporta tokens v1 e v2 do Microsoft Entra ID
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Mostra detalhes do token e logs de validação (apenas em desenvolvimento)
        IdentityModelEventSource.ShowPII = true;

        // Obter configurações
        var tenantId = config["Authentication:TenantId"] 
            ?? throw new InvalidOperationException("TenantId não configurado");
        var audience = config["Authentication:Audience"] 
            ?? throw new InvalidOperationException("Audience não configurado");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Endpoint v2.0 do Entra ID (padrão)
                options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                options.Audience = audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    NameClaimType = "name",
                    // 🔹 Aceita tokens da v1 e v2
                    ValidIssuers = new[]
                    {
                        $"https://login.microsoftonline.com/{tenantId}/v2.0",  // v2.0
                        $"https://sts.windows.net/{tenantId}/"                  // v1.0
                    },
                    ValidAudiences = new[]
                    {
                        audience,                                               // v2.0 format
                        tenantId.Replace("https://login.microsoftonline.com/", "").Replace("/v2.0", "") // v1.0 format
                    }
                };

                // Eventos para debug detalhado
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("❌ Authentication failed:");
                        Console.WriteLine(context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validado com sucesso:");
                        Console.WriteLine($"Claims ({context.Principal?.Claims.Count()}):");
                        if (context.Principal != null)
                        {
                            foreach (var claim in context.Principal.Claims)
                            {
                                Console.WriteLine($"   - {claim.Type}: {claim.Value}");
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("❌ Challenge de autenticação:");
                        Console.WriteLine(context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Console.WriteLine("🚫 Forbidden (sem autorização):");
                        Console.WriteLine($"   Principal: {context.HttpContext.User?.Identity?.Name}");
                        Console.WriteLine($"   Roles: {string.Join(", ", context.HttpContext.User?.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value) ?? new List<string>())}");
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
