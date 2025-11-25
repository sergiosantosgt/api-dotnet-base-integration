namespace BaseIntegrationApi.Endpoints;

/// <summary>
/// Endpoint de previsão do tempo
/// </summary>
public static class WeatherForecastEndpoint
{
    private static readonly string[] Summaries = 
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", 
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static WeatherForecast[] GetForecast()
    {
        return Enumerable.Range(1, 5)
            .Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))
            .ToArray();
    }
}

/// <summary>
/// Modelo de resposta para previsão do tempo
/// </summary>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Temperatura em Fahrenheit (calculada)
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
