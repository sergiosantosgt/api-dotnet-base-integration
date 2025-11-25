namespace BaseIntegrationApi.Endpoints;

/// <summary>
/// Endpoint de debug - mostra informações do token autenticado
/// Apenas disponível em desenvolvimento
/// </summary>
public static class DebugEndpoint
{
    public static IResult GetClaims(HttpContext context)
    {
        var user = context.User;
        var response = new ClaimsDebugResponse
        {
            IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
            Name = user.Identity?.Name,
            Claims = user.Claims
                .Select(c => new ClaimInfo { Type = c.Type, Value = c.Value })
                .ToList(),
            Roles = user.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(c => c.Value)
                .ToList()
        };

        return Results.Ok(response);
    }
}

/// <summary>
/// Resposta de debug com informações do token
/// </summary>
public class ClaimsDebugResponse
{
    /// <summary>
    /// Indica se o usuário está autenticado
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Nome do usuário (do claim 'name')
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Lista de todas as claims do token
    /// </summary>
    public List<ClaimInfo> Claims { get; set; } = new();

    /// <summary>
    /// Lista de roles do usuário
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// Informação de uma claim individual
/// </summary>
public class ClaimInfo
{
    /// <summary>
    /// Tipo da claim
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Valor da claim
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
