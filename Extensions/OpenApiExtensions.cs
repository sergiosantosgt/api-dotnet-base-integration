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

            // Endpoint de debug que lista as rotas/endpoints registrados (apenas dev)
            app.MapGet("/__endpoints", () =>
            {
                var ds = ((IEndpointRouteBuilder)app).DataSources;
                var list = ds.SelectMany(s => s.Endpoints)
                    .Select(e => new
                    {
                        DisplayName = e.DisplayName,
                        Route = e is RouteEndpoint re ? re.RoutePattern.RawText : e.ToString()
                    })
                    .OrderBy(x => x.Route)
                    .ToList();

                return Results.Ok(list);
            });
        }

        return app;
    }
}
