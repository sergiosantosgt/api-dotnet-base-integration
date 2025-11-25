using BaseIntegrationApi.Endpoints;

namespace BaseIntegrationApi.Extensions;

/// <summary>
/// Extensões para configuração de endpoints
/// Centraliza o mapeamento de todas as rotas da aplicação
/// </summary>
public static class EndpointExtensions
{
    public static WebApplication MapCustomEndpoints(this WebApplication app)
    {
        // Mapear endpoints por funcionalidade
        app.MapWeatherForecastEndpoints();
        app.MapHealthCheckEndpoints();
        app.MapDebugEndpoints();

        return app;
    }

    /// <summary>
    /// Endpoints relacionados a previsão do tempo
    /// </summary>
    private static void MapWeatherForecastEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/weather")
            .WithTags("Weather");

        group.MapGet("/forecast", WeatherForecastEndpoint.GetForecast)
            .RequireAuthorization("RequireBackendAccess")
            .WithName("GetWeatherForecast")
            .WithDescription("Obtém previsão do tempo para os próximos 5 dias")
            .Produces<WeatherForecast[]>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    /// <summary>
    /// Endpoints de health check
    /// </summary>
    private static void MapHealthCheckEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/health")
            .WithTags("Health");

        group.MapGet("", HealthCheckEndpoint.Check)
            .WithName("HealthCheck")
            .WithDescription("Verifica se a aplicação está saudável")
            .Produces<HealthCheckResponse>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Endpoints de debug (apenas em desenvolvimento)
    /// </summary>
    private static void MapDebugEndpoints(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        var group = app.MapGroup("/api/v1/debug")
            .WithTags("Debug");

        group.MapGet("/claims", DebugEndpoint.GetClaims)
            .RequireAuthorization()
            .WithName("DebugClaims")
            .WithDescription("Mostra todas as claims do token autenticado")
            .Produces<ClaimsDebugResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}
