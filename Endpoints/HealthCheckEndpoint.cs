namespace BaseIntegrationApi.Endpoints;

/// <summary>
/// Endpoint de health check
/// </summary>
public static class HealthCheckEndpoint
{
    public static HealthCheckResponse Check()
    {
        return new HealthCheckResponse
        {
            Status = "healthy",
            Time = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        };
    }
}

/// <summary>
/// Resposta do health check
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Status da aplicação
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Hora UTC da verificação
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Ambiente de execução
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}
