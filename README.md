# 🛒 EcommerceQueue - Microservices Architecture

Sistema de e-commerce distribuído usando **Clean Architecture**, **CQRS**, **Event-Driven Architecture** e **.NET 10**.

## 🏗️ Arquitetura

```
┌─────────────────┐    Events    ┌─────────────────┐    Events    ┌─────────────────┐
│   Orders API    │ ──────────── │   Payment API   │ ──────────── │   Stock API     │
│   Port: 5010    │              │   Port: 5020    │              │   Port: 5030    │
└─────────────────┘              └─────────────────┘              └─────────────────┘
         │                               │                               │
         └───────────────────────────────┼───────────────────────────────┘
                                         │
                            ┌─────────────────┐
                            │   RabbitMQ      │
                            │   Port: 5672    │
                            └─────────────────┘
```

## 🚀 Tecnologias

- **.NET 10** - Framework principal
- **Clean Architecture** - Separação de responsabilidades
- **CQRS + MediatR** - Command Query Responsibility Segregation
- **MassTransit + RabbitMQ** - Message broker para eventos
- **Entity Framework Core** - ORM para persistência
- **AutoMapper** - Mapeamento entre objetos
- **OpenTelemetry** - Observabilidade e tracing
- **Serilog** - Logging estruturado
- **ASP.NET Core Identity** - Autenticação e autorização
- **Scalar** - Documentação de API interativa

## 📁 Estrutura do Projeto

```
src/
├── BuildingBlocks/           # Componentes compartilhados
│   ├── Messaging/           # Configuração MassTransit e idempotência
│   └── Observability/       # OpenTelemetry e Serilog
├── Contracts/
│   └── Events/             # Eventos compartilhados entre serviços
└── Services/
    ├── Orders/             # Serviço de pedidos
    │   ├── Orders.API/     # API endpoints e controllers
    │   ├── Application/    # Casos de uso (CQRS handlers)
    │   ├── Domain/         # Entidades e regras de negócio
    │   ├── Infrastructure/ # Persistência e implementações
    │   └── ServiceDefaults/# Configurações padrão (.NET Aspire)
    ├── Payment/            # Serviço de pagamentos
    └── Stock/              # Serviço de estoque
```

## 🔧 Pré-requisitos

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **RabbitMQ Server** - [Download](https://www.rabbitmq.com/download.html)
- **SQL Server LocalDB** (incluído no Visual Studio)
- **Redis** (opcional) - Para cache distribuído

## ⚡ Quick Start

### 1. Clone o repositório
```bash
git clone https://github.com/your-repo/EcommerceQueue.git
cd EcommerceQueue
```

### 2. Inicie os serviços de infraestrutura

**RabbitMQ:**
```bash
# Windows (com Docker)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Ou instale nativo: https://www.rabbitmq.com/install-windows.html
```

**Redis (Opcional):**
```bash
docker run -d --name redis -p 6379:6379 redis:alpine
```

### 3. Execute os microserviços

**Terminal 1 - Orders API:**
```bash
cd src/Services/Orders/Orders.API
dotnet run --urls "http://localhost:5010"
```

**Terminal 2 - Payment API:**
```bash
cd src/Services/Payment/Payment.API
dotnet run --urls "http://localhost:5020"
```

**Terminal 3 - Stock API:**
```bash
cd src/Services/Stock/Stock.API
dotnet run --urls "http://localhost:5030"
```

## 🌐 URLs dos Serviços

| Serviço | URL Base | Documentação |
|---------|----------|--------------|
| Orders | `http://localhost:5010` | `http://localhost:5010/scalar` |
| Payment | `http://localhost:5020` | `http://localhost:5020/scalar` |
| Stock | `http://localhost:5030` | `http://localhost:5030/scalar` |
| RabbitMQ Management | `http://localhost:15672` | user/pass: `guest/guest` |

## 🧪 Testando a Aplicação

### Criar um Pedido

**Endpoint:** `POST http://localhost:5010/api/orders`

**Payload:**
```json
{
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "totalAmount": 199.99,
  "items": [
    {
      "productId": "456e7890-e89b-12d3-a456-426614174001",
      "productName": "Laptop",
      "quantity": 1,
      "price": 199.99
    }
  ]
}
```

**Resposta esperada:**
```json
{
  "orderId": "175961bf-2976-4525-badd-a86c0973ff69"
}
```

### Fluxo Completo de Eventos

1. **Orders API** recebe POST `/api/orders`
2. **Orders API** publica `OrderCreatedEvent` no RabbitMQ
3. **Payment API** consome evento e processa pagamento
4. **Stock API** consome evento e reserva estoque
5. Logs aparecem em todos os serviços mostrando o processamento

## 🏛️ Padrões Arquiteturais

### Clean Architecture
- **Domain:** Entidades e regras de negócio puras
- **Application:** Casos de uso e lógica de aplicação
- **Infrastructure:** Implementações de persistência e APIs externas
- **API:** Controllers e configuração web

### CQRS (Command Query Responsibility Segregation)
```csharp
// Command
public record CreateOrderCommand(Guid CustomerId, decimal TotalAmount, List<OrderItemDto> Items);

// Handler
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // Implementação do caso de uso
    }
}
```

### Event-Driven Architecture
```csharp
// Evento
public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { g; init; }
    // ...
}

// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        // Processar evento
    }
}
```

## 🔍 Observabilidade

### Logs Estruturados (Serilog)
```json
{
  "timestamp": "2026-04-03T14:24:13.123Z",
  "level": "Information",
  "service": "Orders.API",
  "message": "Order {OrderId} created successfully",
  "orderId": "175961bf-2976-4525-badd-a86c0973ff69"
}
```

### Tracing (OpenTelemetry)
- Rastreamento distribuído entre serviços
- Correlação de requests através de trace IDs
- Métricas de performance automáticas

## 🛠️ Comandos Úteis

```bash
# Build todos os projetos
dotnet build

# Limpar cache
dotnet clean

# Restaurar dependências
dotnet restore

# Parar build server
dotnet build-server shutdown

# Build específico
dotnet build src/Services/Orders/Orders.API/Orders.API.csproj

# Executar com hot reload
dotnet watch run --urls "http://localhost:5010"
```

## 📊 Configuração de Ambiente

### appsettings.Development.json (exemplo Orders)
```json
{
  "ConnectionStrings": {
    "EcommerceDb": "Server=(localdb)\\mssqllocaldb;Database=EcommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "Redis": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Services": {
    "PaymentApi": "http://localhost:5020",
    "StockApi": "http://localhost:5030"
  }
}
```

## 🚨 Troubleshooting

### RabbitMQ Connection Issues
```bash
# Verificar se RabbitMQ está rodando
docker ps | grep rabbitmq

# Reiniciar RabbitMQ
docker restart rabbitmq
```

### Build Errors
```bash
# Limpar tudo e rebuildar
dotnet clean
dotnet build-server shutdown
dotnet restore
dotnet build
```

### Port Conflicts
```bash
# Verificar portas em uso
netstat -ano | findstr :5010
```

## 🤝 Contribuindo

1. Fork o projeto
2. Crie sua branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🎯 Próximos Passos

- [ ] Implementar autenticação JWT
- [ ] Adicionar testes unitários e de integração
- [ ] Configurar CI/CD pipeline
- [ ] Implementar circuit breaker pattern
- [ ] Adicionar métricas customizadas
- [ ] Deploy com Docker Compose
- [ ] Configurar ambiente Kubernetes

---

**Desenvolvido usando .NET 10 e Clean Architecture**