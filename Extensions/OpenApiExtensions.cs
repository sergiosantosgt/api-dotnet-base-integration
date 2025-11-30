namespace BaseIntegrationApi.Extensions;

/// <summary>
/// Extensões para configuração do OpenAPI/Swagger
/// Customiza documentação da API
/// </summary>
public static class OpenApiExtensions
{
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Base Integration API",
                    Version = "v1",
                    Description = "API de integração com autenticação JWT (v1 e v2 do Microsoft Entra ID)",
                    Contact = new()
                    {
                        Name = "Sergio Santos",
                        Email = "sergiorobsantos@gmail.com"
                    }
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication MapCustomOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}
