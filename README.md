# 🚀 Base Integration API

API .NET 10 para integrações com autenticação JWT (Microsoft Entra ID v1 e v2), desenvolvida com padrões de arquitetura profissionais.

## 📋 Sobre o Projeto

Esta é uma **API base pronta para usar** com:
- ✅ Autenticação JWT (Microsoft Entra ID)
- ✅ Autorização baseada em Roles (RBAC)
- ✅ Arquitetura limpa e escalável
- ✅ Documentação automática (OpenAPI/Swagger)
- ✅ Exemplos prontos para implementação

---

## 🚀 Quick Start

### Pré-requisitos

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Git](https://git-scm.com/) 
- Editor: [VS Code](https://code.visualstudio.com/) ou [Visual Studio](https://visualstudio.microsoft.com/)

### Instalação e Execução

```bash
# 1. Clonar o repositório
git clone https://github.com/sergiosantosgt/api-dotnet-base-integration.git
cd api-dotnet-base-integration

# 2. Restaurar dependências
dotnet restore

# 3. Compilar o projeto
dotnet build

# 4. Executar
dotnet run
```

A aplicação iniciará em: **http://localhost:5023**

### Verificar se está rodando

```bash
# Health check (público - sem autenticação)
curl http://localhost:5023/api/v1/health
```

**Resposta esperada:**
```json
{
  "status": "healthy",
  "time": "2025-11-25T06:19:43.827Z",
  "environment": "Development"
}
```

---

## 🏗️ Arquitetura

### Estrutura de Pastas

```
├── Program.cs                          ← Configuração principal
│
├── Extensions/                         ← Extensões de configuração
│   ├── AuthenticationExtensions.cs     • JWT (v1 e v2)
│   ├── AuthorizationExtensions.cs      • Policies/RBAC
│   ├── EndpointExtensions.cs           • Route mapping
│   └── OpenApiExtensions.cs            • Swagger config
│
├── Endpoints/                          ← Handlers de rotas
│   ├── WeatherForecastEndpoint.cs      • GET /api/v1/weather/forecast
│   ├── HealthCheckEndpoint.cs          • GET /api/v1/health
│   └── DebugEndpoint.cs                • GET /api/v1/debug/claims
│
└── appsettings.json                    ← Configurações
```

### Fluxo de Requisição

```
HTTP Request
    ↓
JWT Bearer Token (Validação)
    ├─ Verifica assinatura
    ├─ Valida issuer (v1 ou v2)
    └─ Valida audience
    ↓
Autorização (Policy Check)
    └─ Verifica roles
    ↓
Endpoint Handler
    └─ Executa lógica
    ↓
JSON Response
```

---

## 🔐 Autenticação e Autorização

### Configuração

O projeto usa **JWT Bearer Token** com **Microsoft Entra ID**:

```json
{
  "Authentication": {
    "TenantId": "a316f897-2d81-4fe1-ae46-5431a84d8df0",
    "Audience": "api://2668672d-7fee-4611-aeb5-bc87f5e84102"
  }
}
```

### Suporte de Versões

| Versão | Endpoint | Audience |
|--------|----------|----------|
| **v2.0** | `login.microsoftonline.com/{tenantId}/v2.0` | `api://...` |
| **v1.0** | `sts.windows.net/{tenantId}/` | `{uuid}` |

A API **aceita tokens das duas versões**.

### Policies Disponíveis

| Policy | Descrição | Uso |
|--------|-----------|-----|
| `RequireBackendAccess` | Exige role `Backend.Access` | `.RequireAuthorization("RequireBackendAccess")` |

---

## 📍 Endpoints

### 🟢 Públicos (Sem autenticação)

#### Health Check
```bash
GET /api/v1/health

Resposta (200):
{
  "status": "healthy",
  "time": "2025-11-25T06:19:43.827Z",
  "environment": "Development"
}
```

### 🔴 Protegidos (Requer token + role)

#### Previsão do Tempo
```bash
GET /api/v1/weather/forecast

Headers:
Authorization: Bearer {seu_token_jwt}

Resposta (200):
[
  {
    "date": "2025-11-26",
    "temperatureC": 20,
    "summary": "Mild",
    "temperatureF": 68
  },
  ...
]
```

#### Debug de Claims (Desenvolvimento)
```bash
GET /api/v1/debug/claims

Headers:
Authorization: Bearer {seu_token_jwt}

Resposta (200):
{
  "isAuthenticated": true,
  "name": "user@example.com",
  "claims": [
    { "type": "oid", "value": "..." },
    { "type": "name", "value": "..." }
  ],
  "roles": ["Backend.Access"]
}
```

---

## 🧪 Testando com Token

### 1. Obter um Token JWT

Você precisa obter um token JWT válido do seu Microsoft Entra ID:

```bash
# Exemplos (use seu tenant ID, client ID e credentials)
curl -X POST https://login.microsoftonline.com/YOUR_TENANT_ID/oauth2/v2.0/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=YOUR_CLIENT_ID&client_secret=YOUR_SECRET&scope=api://YOUR_AUDIENCE/.default&grant_type=client_credentials"
```

### 2. Testar com Token

```bash
# Salvar o token em variável
TOKEN="seu_token_jwt_aqui"

# Testar endpoint protegido
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5023/api/v1/weather/forecast

# Debug de claims
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5023/api/v1/debug/claims
```

### 3. Sem Token (Deve retornar 401)

```bash
curl http://localhost:5023/api/v1/weather/forecast
# Resposta: 401 Unauthorized
```

---

## 🔍 Logs e Debug

A aplicação exibe logs detalhados no console:

```
✅ Token validado com sucesso:
Claims (15):
   - oid: 12345...
   - name: user@example.com
   - roles: Backend.Access
   - ...

🚫 Forbidden (sem autorização):
   Principal: user@example.com
   Roles: [vazio ou role incorreta]

❌ Authentication failed:
   [detalhes do erro]
```

---

## 📦 Dependências Principais

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.0" />
<PackageReference Include="Microsoft.Identity.Web" Version="4.1.1" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
```

---

## 🛠️ Adicionando um Novo Endpoint

### 1. Criar Handler (`Endpoints/ProdutoEndpoint.cs`)

```csharp
namespace BaseIntegrationApi.Endpoints;

public static class ProdutoEndpoint
{
    public static Produto[] GetProdutos()
    {
        return new[] { new Produto { Id = 1, Nome = "Notebook" } };
    }
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
}
```

### 2. Mapear Rota (`Extensions/EndpointExtensions.cs`)

```csharp
private static void MapProdutoEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/produtos").WithTags("Produtos");
    
    group.MapGet("", ProdutoEndpoint.GetProdutos)
        .RequireAuthorization("RequireBackendAccess")
        .WithName("GetProdutos")
        .Produces<Produto[]>(StatusCodes.Status200OK);
}
```

### 3. Registrar (`MapCustomEndpoints()`)

```csharp
public static WebApplication MapCustomEndpoints(this WebApplication app)
{
    app.MapWeatherForecastEndpoints();
    app.MapHealthCheckEndpoints();
    app.MapDebugEndpoints();
    app.MapProdutoEndpoints();  // ← Adicionar
    
    return app;
}
```

**Pronto! Endpoint criado em 5 minutos.**

---

## 📚 Documentação Adicional

Para entender melhor a arquitetura e padrões implementados:

- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Guia de arquitetura detalhado
- **[EXAMPLES.md](./EXAMPLES.md)** - 6 exemplos práticos de endpoints
- **[PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md)** - Diagramas visuais
- **[INDEX.md](./INDEX.md)** - Índice completo de documentação

---

## ⚙️ Configuração de Variáveis de Ambiente

Para desenvolvimento local, configure em `appsettings.Development.json`:

```json
{
  "Authentication": {
    "TenantId": "seu_tenant_id",
    "Audience": "api://seu_client_id"
  }
}
```

Para produção, configure as variáveis de ambiente:

```bash
Authentication__TenantId=seu_tenant_id
Authentication__Audience=api://seu_client_id
```

---

## 🐛 Troubleshooting

### Erro: "Address already in use"
Outra instância está rodando na porta 5023:

```bash
# Matar processo
killall dotnet

# Ou especificar outra porta
dotnet run --urls "http://localhost:5024"
```

### Erro: 401 Unauthorized (sem autenticação)
- Verifique se o token é válido
- Verifique se o token não expirou
- Verifique `TenantId` e `Audience` em `appsettings.json`

### Erro: 403 Forbidden (sem autorização)
- O token é válido, mas falta a role `Backend.Access`
- Verifique os logs do endpoint `/api/v1/debug/claims`
- Adicione a role ao usuário no Microsoft Entra ID

### Logs confusos?
Aumente verbosidade em `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  }
}
```

---

## 📄 Licença

Este projeto é de código aberto e livre para usar.

---

## 👤 Autor

Desenvolvido como base de integração profissional.

**Versão:** 1.0 
