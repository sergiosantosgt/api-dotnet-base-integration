using BaseIntegrationApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configuração de Serviços
// ============================================

// Autenticação JWT (v1 e v2 do Microsoft Entra ID)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Autorização com policies
builder.Services.AddCustomAuthorization();

// OpenAPI/Swagger
builder.Services.AddCustomOpenApi();

// CORS (Cross-Origin Resource Sharing)
builder.Services.AddCustomCors();

// ============================================
// Configuração do Pipeline
// ============================================

var app = builder.Build();

// CORS deve estar antes de autenticação
app.UseCustomCors();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// OpenAPI/Swagger
app.MapCustomOpenApi();

// Endpoints
app.MapCustomEndpoints();

// ============================================
// Iniciar aplicação
// ============================================

app.Run();

