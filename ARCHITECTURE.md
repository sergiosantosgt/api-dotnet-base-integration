# 🏗️ Arquitetura do Projeto

## 📋 Resumo

Este projeto foi refatorado seguindo **padrões de mercado** para ter uma estrutura profissional, escalável e fácil de manter.

---

## 📁 Estrutura de Pastas

```
├── Extensions/                          # 🔧 Extensões para configuração
│   ├── AuthenticationExtensions.cs      # JWT (v1 e v2 Microsoft Entra ID)
│   ├── AuthorizationExtensions.cs       # Policies e autorização
│   ├── EndpointExtensions.cs            # Mapeamento de rotas
│   └── OpenApiExtensions.cs             # Configuração do Swagger/OpenAPI
│
├── Endpoints/                           # 🛣️ Handlers de rotas
│   ├── WeatherForecastEndpoint.cs       # Endpoint de previsão do tempo
│   ├── HealthCheckEndpoint.cs           # Health check da aplicação
│   └── DebugEndpoint.cs                 # Debug de claims (dev only)
│
├── Program.cs                           # 📝 Configuração principal (limpo e claro)
├── appsettings.json                     # ⚙️ Configurações da aplicação
└── BaseIntegrationApi.csproj            # 📦 Definição do projeto
```

---

## 🔐 Autenticação & Autorização

### Características

✅ **JWT Bearer Token**
- Suporta tokens da v1 e v2 do Microsoft Entra ID
- Validação automática de issuer, audience e assinatura

✅ **Roles-Based Access Control (RBAC)**
- Policy: `RequireBackendAccess` - exige role `Backend.Access`
- Fácil adicionar mais policies conforme necessário

✅ **Debug**
- Endpoint `/api/v1/debug/claims` para inspecionar claims do token
- Logs detalhados no console (desenvolvimento)

### Endpoints

| Rota | Método | Auth | Descrição |
|------|--------|------|-----------|
| `/api/v1/weather/forecast` | GET | ✅ Backend.Access | Previsão do tempo |
| `/api/v1/health` | GET | ❌ Público | Health check |
| `/api/v1/debug/claims` | GET | ✅ Autenticado | Mostra claims do token |

---

## 🏛️ Padrões Implementados

### 1️⃣ **Extension Methods Pattern**
Separação clara de responsabilidades usando extension methods:
- Fácil ler o `Program.cs`
- Cada extensão cuida de seu domínio
- Reutilizável em múltiplos projetos

### 2️⃣ **Endpoint Handler Pattern**
Cada rota tem seu handler estático em `Endpoints/`:
- Lógica isolada
- Fácil testar
- Models de resposta próximos do handler

### 3️⃣ **Configuration by Convention**
Usa `appsettings.json` para configurações:
```json
{
  "Authentication": {
    "TenantId": "...",
    "Audience": "..."
  }
}
```

### 4️⃣ **API Versioning**
Rotas seguem padrão RESTful com versão:
- `/api/v1/weather/forecast`
- `/api/v1/health`
- `/api/v1/debug/claims`

### 5️⃣ **Minimal APIs**
Usa o modelo minimal do ASP.NET Core 10:
- Menos boilerplate
- Performance melhor
- Mais moderno

---

## 🚀 Como Usar

### Adicionar um Novo Endpoint

1. **Criar handler em `Endpoints/NovoEndpoint.cs`:**
```csharp
namespace BaseIntegrationApi.Endpoints;

public static class NovoEndpoint
{
    public static MinhaResposta Handler(HttpContext context)
    {
        // Lógica aqui
        return new MinhaResposta();
    }
}

public class MinhaResposta
{
    public string Message { get; set; } = "OK";
}
```

2. **Mapear em `Extensions/EndpointExtensions.cs`:**
```csharp
private static void MapNovoEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/novo")
        .WithTags("Novo");

    group.MapGet("", NovoEndpoint.Handler)
        .WithName("GetNovo")
        .WithDescription("Descrição do endpoint")
        .Produces<MinhaResposta>(StatusCodes.Status200OK);
}
```

3. **Chamar em `MapCustomEndpoints`:**
```csharp
public static WebApplication MapCustomEndpoints(this WebApplication app)
{
    app.MapWeatherForecastEndpoints();
    app.MapHealthCheckEndpoints();
    app.MapDebugEndpoints();
    app.MapNovoEndpoints();  // ← Adicionar
    
    return app;
}
```

### Adicionar uma Nova Policy

Editar `Extensions/AuthorizationExtensions.cs`:
```csharp
options.AddPolicy("RequireAdmin", policy =>
    policy.RequireRole("Admin"));
```

Usar no endpoint:
```csharp
.RequireAuthorization("RequireAdmin")
```

---

## 🧪 Testando

### Health Check (Público)
```bash
curl http://localhost:5000/api/v1/health
```

### Previsão do Tempo (Com Token)
```bash
curl -H "Authorization: Bearer SEU_TOKEN" \
  http://localhost:5000/api/v1/weather/forecast
```

### Debug de Claims (Com Token)
```bash
curl -H "Authorization: Bearer SEU_TOKEN" \
  http://localhost:5000/api/v1/debug/claims
```

---

## 📦 Dependências

- **Microsoft.AspNetCore.Authentication.JwtBearer**: Autenticação JWT
- **Microsoft.IdentityModel.Tokens**: Validação de tokens
- **Microsoft.Identity.Web**: Integração com Microsoft Entra ID
- **Microsoft.AspNetCore.OpenApi**: Documentação automática

---

## ✅ Checklist de Boas Práticas

- ✅ Configuração centralizada
- ✅ Separação de responsabilidades
- ✅ DRY (Don't Repeat Yourself)
- ✅ Logging detalhado
- ✅ XML documentation comments
- ✅ Versioning de API
- ✅ RESTful conventions
- ✅ Minimal APIs
- ✅ Suporte v1 e v2 JWT
- ✅ RBAC com policies

---

## 🎯 Próximos Passos

1. Substituir `ShowPII = true` em produção por configuração condicional
2. Adicionar logging estruturado (Serilog)
3. Adicionar healthchecks mais robustos (DB, dependências)
4. Adicionar testes unitários
5. Considerar implementar CQRS para lógica mais complexa

