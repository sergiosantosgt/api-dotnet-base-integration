namespace BaseIntegrationApi.Extensions;

/// <summary>
/// Extensões para configuração de CORS (Cross-Origin Resource Sharing)
/// Permite requisições de origens específicas (ex: localhost:3000)
/// </summary>
public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("LocalhostPolicy", builder =>
            {
                builder
                    .WithOrigins(
                        "http://localhost:3000",
                        "http://127.0.0.1:3000",
                        "http://localhost:5023",
                        "http://127.0.0.1:5023"
                    )
                    .AllowAnyMethod()           // GET, POST, PUT, DELETE, etc.
                    .AllowAnyHeader()           // Qualquer header
                    .AllowCredentials();        // Permite cookies/auth headers
            });

            // Policy alternativa: permitir qualquer origem (apenas desenvolvimento)
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static WebApplication UseCustomCors(this WebApplication app)
    {
        // Aplicar CORS policy baseado no ambiente
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("LocalhostPolicy");
        }
        else
        {
            // Em produção, opcionalmente aplicar uma policy mais restritiva
            // app.UseCors("LocalhostPolicy");
        }

        return app;
    }
}
