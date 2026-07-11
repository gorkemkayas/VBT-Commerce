# Backend Architecture Documentation

## 1. General Architecture

- **Modular Monolith** structure will be used.
- **Domain Driven Design (DDD)** principles will be applied.
- The project will be deployed as a single process, but modules will be separated by clear boundaries.

---

## 2. Database Structure

- A **single physical database** will be used.
- Each module will have **its own schema** (e.g. `order_schema`, `inventory_schema`, `payment_schema`).
- Each module manages its own **DbContext**; modules never directly access another module's DbContext.
- **Shared transactions / TransactionScope across modules will not be used.** Each module manages only its own transaction.
- Cross-module consistency will be handled with a **hybrid** approach: operations that are critical / require an immediate result use synchronous Integration Calls, while non-critical side effects are handled via the Outbox Pattern + Domain Events (see Section 6).

```
AppDb (single physical DB)
├── order_schema
├── inventory_schema
└── payment_schema
```

---

## 3. Inter-Module Communication

Modules communicate with each other **not directly, but through contracts.**

- **contracts/** → The public API (interface) that a module exposes to the OUTSIDE.
- **integrations/** → The layer through which a module obtains what it needs from OTHER modules (consumes another module's contract).

```
ModuleA/
  ├── contracts/         ← Services ModuleA exposes to the outside
  │    └── IModuleAService.ts
  │
  └── integrations/       ← Services ModuleA consumes from other modules
       └── IModuleBIntegration.cs (uses ModuleB's contract)
```

**Rule:** A module never directly accesses another module's repository or DbContext. It only communicates through contracts/integrations.

---

## 4. CQRS Pattern

- **Command Query Responsibility Segregation** will be used.
- Write (Command) and Read (Query) operations will be separated.
- Command/Query Handlers will live in the **Application layer**.
- **Mediator:** MediatR will be used.

```
Module/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   └── Exceptions/
│
├── Application/
│   ├── Commands/
│   │   └── CreateOrder/
│   │       ├── CreateOrderCommand.cs
│   │       ├── CreateOrderCommandHandler.cs
│   │       └── CreateOrderCommandValidator.cs
│   │
│   ├── Queries/
│   │   └── GetOrderById/
│   │       ├── GetOrderByIdQuery.cs
│   │       └── GetOrderByIdQueryHandler.cs
│   │
│   ├── EventHandlers/
│   │   └── OrderCreatedEventHandler.cs
│   │
│   └── Behaviors/
│       ├── LoggingBehavior.cs
│       ├── ExceptionHandlingBehavior.cs
│       ├── ValidationBehavior.cs
│       └── TransactionBehavior.cs
│
└── Infrastructure/
```

### Marker Interfaces

```csharp
public interface ICommand<TResponse> : IRequest<TResponse> { }
public interface IQuery<TResponse> : IRequest<TResponse> { }
```

---

## 5. Cross-Cutting Concerns: Pipeline Behaviors (Execute Around Pattern)

Repetitive concerns such as logging, exception handling, validation, and transactions are not written manually in every Handler. Instead, they are automatically wrapped around the Handler (execute around) via the **MediatR Pipeline Behavior** mechanism.

### Pipeline Order

```
Request
  ↓
1. LoggingBehavior              (outermost, logs everything)
  ↓
2. ExceptionHandlingBehavior    (catches, logs, and rethrows errors)
  ↓
3. ValidationBehavior           (validation via FluentValidation)
  ↓
4. TransactionBehavior          (Commands only, single-module transaction)
  ↓
5. Handler                      (pure business logic)
  ↓
Response
```

### Why Separate Behaviors?

- Single Responsibility Principle — each is responsible for one concern only.
- Independently testable, can be easily added or removed when needed.
- It becomes impossible to forget these behaviors when writing a new handler (centralized, guaranteed application).

### Validation

- **FluentValidation** will be used.
- A `Validator` class will be written for each Command/Query; `ValidationBehavior` runs them automatically.

### Transaction Behavior — Scope

> The scope of Transaction Behavior is **always limited to a single module** — it never performs a shared transaction across modules.

- Each Command operates only within its **own module's** DbContext and transaction.
- Even if a Command Handler calls `SaveChangesAsync()` multiple times (e.g. entity save + OutboxMessage save in the same schema), all of them commit/rollback atomically within this single transaction.
- **Shared transactions / TransactionScope across modules are not used.** Cross-module consistency is achieved via the hybrid approach described in Section 6 (synchronous Integration Call + Outbox/Event).
- Only activates for `ICommand` requests; no transaction is opened for `IQuery`.

```csharp
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICommand) return await next();

        // Single DbContext, single transaction (within module boundary)
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var response = await next();
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
```

---

## 6. Cross-Module Consistency: Hybrid Approach (Sync Call + Outbox Pattern)

Instead of a single strategy for cross-module operations, **two different approaches** are used depending on the nature of the operation. The decision criterion is simple:

> **"Must the result of this operation return to the user immediately / should the main operation not happen at all if this fails?"**
> - **Yes** → Synchronous Integration Call
> - **No** (can be delayed, non-critical side effect) → Outbox Pattern + Domain Event

### 6.1. Synchronous Integration Call (Critical Path)

Used for checks that require an immediate result and directly affect the main operation's flow. Example: stock check/reservation while creating an Order.

```csharp
public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken ct)
{
    // 1. FIRST: Synchronous stock check + reservation (via Contracts/Integrations)
    var reservationResult = await _inventoryIntegration.ReserveStockAsync(request.Items, ct);

    if (!reservationResult.IsSuccess)
        throw new DomainException($"Insufficient stock: {reservationResult.ErrorMessage}");
        // ← Order is never created; the user sees the error IMMEDIATELY

    // 2. If stock was reserved, the Order is created (its own transaction, via TransactionBehavior)
    var order = Order.Create(request.CustomerId, request.Items);
    _orderContext.Orders.Add(order);

    // 3. Non-critical side effect: an event is written to the Outbox in the same transaction (see 6.2)
    _orderContext.OutboxMessages.Add(new OutboxMessage(new OrderCreatedEvent(order.Id, order.CustomerId)));

    await _orderContext.SaveChangesAsync(ct);
    return CreateOrderResult.From(order);
}
```

### Compensating Action — Hybrid Approach (Rollback + ExpiresAt)

If the Order record fails after stock has been reserved (e.g. an unexpected error), the reservation must be rolled back. Instead of relying on a single mechanism, **a two-layer safety net** is applied:

**1. Primary mechanism — Synchronous Rollback Call:**

```csharp
public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken ct)
{
    var reservationResult = await _inventoryIntegration.ReserveStockAsync(request.Items, ct);

    if (!reservationResult.IsSuccess)
        throw new DomainException($"Insufficient stock: {reservationResult.ErrorMessage}");

    try
    {
        var order = Order.Create(request.CustomerId, request.Items);
        _orderContext.Orders.Add(order);
        _orderContext.OutboxMessages.Add(new OutboxMessage(new OrderCreatedEvent(order.Id)));

        await _orderContext.SaveChangesAsync(ct);
        return CreateOrderResult.From(order);
    }
    catch (Exception)
    {
        // Order save failed, release the reservation
        await _inventoryIntegration.ReleaseStockAsync(reservationResult.ReservationId, ct);
        throw; // The exception is not swallowed; pipeline behaviors (Transaction/Exception/Logging) continue their normal flow
    }
}
```

**2. Safety net — `ExpiresAt` field (in case the rollback call also fails):**

The rollback call itself can also fail (e.g. due to a network interruption or a temporary service outage). To prevent stock from appearing "reserved" forever in this case, an expiration timestamp is added to the `StockReservation` record:

```csharp
public class StockReservation
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiresAt { get; set; }   // e.g. CreatedAt + 30 minutes
    public bool IsConfirmed { get; set; }     // becomes true once the Order is finalized
}
```

The stock availability query already excludes expired reservations — this means **a separate cleanup background service is not needed for now**; the logical protection is enforced at the query level:

```csharp
var availableStock = stock.Quantity -
    await _context.StockReservations
        .Where(r => r.ProductId == productId && !r.IsConfirmed && r.ExpiresAt > DateTime.UtcNow)
        .SumAsync(r => r.Quantity);
```

> **In the future (as scale grows):** If expired "ghost" reservation records accumulate in the table and affect performance, a cleanup background service that physically clears/releases them can be added. Not needed at the MVP stage.

### 6.2. Outbox Pattern + Domain Events (Non-Critical Side Effects)

Used for operations whose delay does not affect the user experience (e.g. sending email/notifications, updating analytics, audit logging). The infrastructure will be **set up now**, so that it is ready to use as soon as a need arises (e.g. once email integration is added).

```
1. OrderModule:
   BEGIN TRANSACTION
       INSERT INTO order_schema.Orders (...)
       INSERT INTO order_schema.OutboxMessages (OrderCreatedEvent)   ← Same transaction!
   COMMIT

2. OutboxProcessorService (BackgroundService, polling):
   - Scans the order_schema.OutboxMessages table (ProcessedAt IS NULL)
   - For each message: mediator.Publish(domainEvent)   ← In-process dispatch
   - Marks it as processed (ProcessedAt = now)

3. NotificationModule (INotificationHandler<OrderCreatedEvent>):
   - Sends email/notification (its own business logic, its own transaction if any)
```

### 6.3. Decision Table (Examples)

| Scenario | Approach | Rationale |
|----------|----------|-----------|
| Stock check / reservation | Synchronous Integration Call | Result must be known immediately; the Order must not be created if it fails |
| Payment check (if any) | Synchronous Integration Call | The user must know immediately if the card is invalid |
| Order confirmation email | Outbox + Event | A few seconds of delay is not a problem |
| Analytics / reporting update | Outbox + Event | Does not need to be instantaneous |

### Technology Decisions

| Topic | Decision | Rationale |
|-------|----------|-----------|
| Event dispatch | **MediatR `Publish` / `INotificationHandler`** (in-process) | Since modules run within the same process, a message broker is not needed |
| Outbox processing | **BackgroundService** (.NET Hosted Service, polling-based) | Simple, requires no extra infrastructure, fits the modular monolith scale |
| Message broker (RabbitMQ/Kafka/Azure Service Bus) | ❌ Not used (for now) | Creates unnecessary network overhead for communication between modules within the same process |
| MassTransit | ❌ Not used (for now) | Its real value lies in message broker integration; an unnecessary dependency here |

### Outbox Table (In Each Module's Own Schema)

```sql
CREATE TABLE {schema}.OutboxMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    EventType NVARCHAR(500) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    ProcessedAt DATETIME2 NULL,
    Error NVARCHAR(MAX) NULL
);
```

### Points to Watch

- **Idempotency:** Event handlers must produce the same result if triggered more than once (e.g. an "already processed?" check). This is useful even on a single instance in retry scenarios, so it should still be implemented.
- **Polling interval:** ~5 seconds is suggested as a starting point, adjustable as needed.
- **Multi-instance scenario:** Since the application **will currently run as a single instance**, this is not a problem, and no extra lock mechanism (`UPDLOCK`/`READPAST`, claim column, etc.) is needed. If the need for horizontal scaling with multiple instances arises in the future, it can be safely solved via EF Core by adding a "claim" column (`ClaimedBy`, `ClaimedAt`).
- **Future compatibility:** If the system moves to a real microservice architecture later, the `mediator.Publish()` call can be replaced with a message broker publish call; the event handler logic remains unchanged.

---

## 7. Middleware vs Pipeline Behavior Distinction

Both are used, but with different responsibilities at different layers:

| Topic | Middleware (HTTP layer) | Pipeline Behavior (Application layer) |
|-------|--------------------------|-----------------------------------------|
| Authentication / Authorization (JWT) | ✅ | — |
| CORS | ✅ | ❌ |
| Exception → HTTP Status Code translation | ✅ | ❌ (only catches, logs, rethrows) |
| Rate Limiting | ✅ | ❌ |
| Business/Command logging | ❌ | ✅ |
| Domain exception handling | ❌ | ✅ |
| Validation (FluentValidation) | ❌ | ✅ |
| Transaction management | ❌ | ✅ |

**Why both are needed:** Commands/Queries can be triggered not just from HTTP but, in the future, from message queues, background jobs, or CLI sources as well. Pipeline Behavior works in all of these scenarios, while Middleware is specific to the HTTP world.

---

## 8. Technology Stack Summary

| Layer/Topic | Technology |
|-------------|------------|
| Mediator | MediatR |
| Validation | FluentValidation |
| ORM | **Entity Framework Core** |
| Outbox Processing | .NET BackgroundService (Hosted Service) |
| Event Dispatch | MediatR `INotificationHandler` (in-process) |
| Message Broker | Not used (for now) |

---

## 9. Open Items / TODO

- [ ] Physical cleanup background service for expired StockReservation records — not needed for now (query-level `ExpiresAt` check is sufficient); can be left for last, added when a need arises
- [ ] Other topics to be discussed will be added here

---

*This document will continue to be updated as the project evolves.*
