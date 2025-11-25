# 💡 Exemplos Práticos de Uso

## Exemplo 1: Adicionar um Endpoint de Produtos

### 1. Criar o handler (`Endpoints/ProductEndpoint.cs`)

```csharp
namespace BaseIntegrationApi.Endpoints;

/// <summary>
/// Endpoint de produtos
/// </summary>
public static class ProductEndpoint
{
    public static IEnumerable<Product> GetAll()
    {
        return new[]
        {
            new Product { Id = 1, Name = "Notebook", Price = 2500 },
            new Product { Id = 2, Name = "Mouse", Price = 50 },
            new Product { Id = 3, Name = "Teclado", Price = 150 }
        };
    }

    public static Product GetById(int id)
    {
        return new Product { Id = id, Name = "Produto X", Price = 100 };
    }
}

/// <summary>
/// Modelo de produto
/// </summary>
public class Product
{
    /// <summary>ID do produto</summary>
    public int Id { get; set; }
    
    /// <summary>Nome do produto</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Preço do produto</summary>
    public decimal Price { get; set; }
}
```

### 2. Mapear as rotas (`Extensions/EndpointExtensions.cs`)

Adicionar este método:
```csharp
/// <summary>
/// Endpoints de produtos
/// </summary>
private static void MapProductEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/products")
        .WithTags("Products");

    group.MapGet("", ProductEndpoint.GetAll)
        .WithName("GetProducts")
        .WithDescription("Lista todos os produtos")
        .Produces<IEnumerable<Product>>(StatusCodes.Status200OK);

    group.MapGet("/{id}", ProductEndpoint.GetById)
        .WithName("GetProductById")
        .WithDescription("Obtém um produto pelo ID")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
```

### 3. Registrar em `MapCustomEndpoints()`

```csharp
public static WebApplication MapCustomEndpoints(this WebApplication app)
{
    app.MapWeatherForecastEndpoints();
    app.MapHealthCheckEndpoints();
    app.MapDebugEndpoints();
    app.MapProductEndpoints();  // ← Adicionar
    
    return app;
}
```

### 4. Testar
```bash
curl http://localhost:5023/api/v1/products
curl http://localhost:5023/api/v1/products/1
```

---

## Exemplo 2: Adicionar uma Policy de Autorização

### 1. Criar a policy (`Extensions/AuthorizationExtensions.cs`)

```csharp
public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireBackendAccess", policy =>
                policy.RequireRole("Backend.Access"));

            // ← Adicionar nova policy
            options.AddPolicy("RequireAdmin", policy =>
                policy.RequireRole("Admin"));

            // Policy que exige uma das múltiplas roles
            options.AddPolicy("RequireManagerOrAdmin", policy =>
                policy.RequireRole("Manager", "Admin"));

            // Policy com claims customizados
            options.AddPolicy("RequireVerified", policy =>
                policy.RequireClaim("verified", "true"));
        });

        return services;
    }
}
```

### 2. Usar no endpoint

```csharp
private static void MapAdminEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/admin")
        .WithTags("Admin");

    group.MapGet("/users", AdminEndpoint.GetUsers)
        .RequireAuthorization("RequireAdmin")  // ← Use a policy
        .WithName("GetUsers");
}
```

---

## Exemplo 3: Endpoint com Body Request

### 1. Criar models

```csharp
namespace BaseIntegrationApi.Endpoints;

/// <summary>
/// Endpoint de usuários
/// </summary>
public static class UserEndpoint
{
    public static CreatedResult Create(CreateUserRequest request)
    {
        var user = new User
        {
            Id = new Random().Next(1000),
            Name = request.Name,
            Email = request.Email
        };

        return new CreatedResult($"/api/v1/users/{user.Id}", user);
    }
}

/// <summary>
/// Request para criar usuário
/// </summary>
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Modelo de usuário
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### 2. Mapear a rota

```csharp
private static void MapUserEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/users")
        .WithTags("Users");

    group.MapPost("", UserEndpoint.Create)
        .RequireAuthorization("RequireBackendAccess")
        .WithName("CreateUser")
        .WithDescription("Cria um novo usuário")
        .Accepts<CreateUserRequest>("application/json")
        .Produces<User>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
}
```

### 3. Testar

```bash
curl -X POST http://localhost:5023/api/v1/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{"name":"João","email":"joao@example.com"}'
```

---

## Exemplo 4: Endpoint com Acesso ao HttpContext

### 1. Handler com HttpContext

```csharp
public static class ProfileEndpoint
{
    public static IResult GetUserProfile(HttpContext context)
    {
        var user = context.User;
        var name = user.Identity?.Name ?? "Unknown";
        var userId = user.FindFirst("oid")?.Value ?? "Unknown";
        
        return Results.Ok(new
        {
            Name = name,
            UserId = userId,
            IsAuthenticated = user.Identity?.IsAuthenticated ?? false
        });
    }
}
```

### 2. Mapear

```csharp
private static void MapProfileEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/v1/profile")
        .WithTags("Profile");

    group.MapGet("", ProfileEndpoint.GetUserProfile)
        .RequireAuthorization()
        .WithName("GetProfile");
}
```

---

## Exemplo 5: Tratamento de Erros

### 1. Handler com validação

```csharp
public static class OrderEndpoint
{
    public static IResult CreateOrder(CreateOrderRequest request)
    {
        // Validação
        if (request.Items == null || request.Items.Count == 0)
        {
            return Results.BadRequest("Pedido deve ter pelo menos um item");
        }

        if (request.Total <= 0)
        {
            return Results.BadRequest("Total deve ser maior que zero");
        }

        // Lógica
        var order = new Order { Id = Guid.NewGuid(), Total = request.Total };

        return Results.Created($"/api/v1/orders/{order.Id}", order);
    }
}

public class CreateOrderRequest
{
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class Order
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
```

---

## Exemplo 6: Endpoint com Dependência Injetada

### 1. Criar um serviço

```csharp
namespace BaseIntegrationApi.Services;

/// <summary>
/// Serviço de dados
/// </summary>
public interface IDataService
{
    Task<string> GetDataAsync();
}

/// <summary>
/// Implementação do serviço
/// </summary>
public class DataService : IDataService
{
    public Task<string> GetDataAsync()
    {
        return Task.FromResult("Dados do serviço");
    }
}
```

### 2. Registrar no Program.cs

```csharp
builder.Services.AddScoped<IDataService, DataService>();
```

### 3. Usar no endpoint

```csharp
public static class DataEndpoint
{
    public static async Task<IResult> GetData(IDataService service)
    {
        var data = await service.GetDataAsync();
        return Results.Ok(new { Data = data });
    }
}
```

---

## 📋 Checklist para Novo Endpoint

- [ ] Criar arquivo em `Endpoints/`
- [ ] Implementar handler estático
- [ ] Implementar models necessários
- [ ] Adicionar XML comments
- [ ] Criar método de mapeamento em `EndpointExtensions.cs`
- [ ] Registrar em `MapCustomEndpoints()`
- [ ] Adicionar `.WithName()` e `.WithDescription()`
- [ ] Adicionar `.Produces()` para documentação
- [ ] Testar com curl ou Postman
- [ ] Adicionar ao OpenAPI documentation

---

## 🧪 Estrutura Padrão de Teste

```bash
# Listar todos
curl http://localhost:5023/api/v1/products

# Obter por ID
curl http://localhost:5023/api/v1/products/1

# Criar (POST)
curl -X POST http://localhost:5023/api/v1/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{"name":"Novo Produto","price":100}'

# Com token
curl -H "Authorization: Bearer SEU_TOKEN" \
  http://localhost:5023/api/v1/weather/forecast
```

---

**Dúvidas? Consulte `ARCHITECTURE.md` para mais detalhes!**

