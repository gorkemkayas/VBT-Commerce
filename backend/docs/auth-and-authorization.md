# Auth & Authorization Documentation

## 1. General Approach

- **Authentication method:** Custom Bearer Token (JWT) + Refresh Token
- **Session/Redis will not be used** — since the role structure is simple (Customer, Admin) and there is no critical requirement such as instant session revocation, Redis-based session management was assessed as unnecessary complexity.
- The Access Token is fully **stateless** (validated on every request without hitting the DB).
- The Refresh Token is stored in **a regular DB table** (not Redis) — this provides logout/revoke capability without extra infrastructure cost.

---

## 2. Roles

| Role | Description |
|------|--------------|
| **Customer** | The user who purchases products and performs cart/order/address/review operations |
| **Admin** | The user who performs store management operations such as products, categories, stock, coupons, and shipping |

> There are only 2 roles for now. If the need arises in the future (e.g. sub-roles like Product Manager, Order Manager), the role-based structure can be extended to policy-based authorization (see Section 6).

---

## 3. Registration / Login Methods

Two methods will be supported together:

### 3.1. Classic Registration/Login with Email / Password
```
- Registration: Email + Password (+ basic info such as First Name, Last Name)
- Passwords are stored hashed (see Section 3.3)
- Login: Email + Password → returns Access Token + Refresh Token
```

### 3.2. Social Login with Google
```
Flow:
1. The user clicks "Sign in with Google"
2. The frontend starts the Google OAuth flow
3. Google returns with an "authorization code"
4. The backend (Identity module) sends this code to Google and 
   retrieves the user's info (email, name, etc.)
5. The backend checks: Does a User with this email already exist?
   - If yes → an Access Token + Refresh Token are issued for that user
   - If no → a new User record is created (passwordless, linked only 
     via social login), then a token is issued
6. The user receives the SYSTEM'S OWN JWT (not Google's token)
```

> **Key design decision:** Google's own token is never held on the frontend/mobile side; it is used only during the authentication step. All other modules (Order, Cart, Review, etc.) only understand the system's own JWT — they are completely unaware of Google's details. This reduces inter-module coupling.

### 3.3. Password Security

```
- Passwords are never stored as plain text
- Hashing algorithm: BCrypt or Argon2 (ASP.NET Core Identity's default 
  PasswordHasher can also be used)
- Users who sign up via social login may have a NULL password field 
  (considered as not having set a password) — a "set password" flow 
  can be added later
```

---

## 4. Token Structure

### 4.1. Access Token (JWT)

```
- Short-lived (suggestion: 15 minutes)
- Stateless — validated without hitting the DB (signature check is sufficient)
- Claims (content):
    - sub (UserId)
    - email
    - role (Customer / Admin)
    - exp (expiration time)
```

### 4.2. Refresh Token

```
- Long-lived (suggestion: 7 days)
- Stored in the DB (identity_schema.RefreshTokens)
- Generated as a random, high-entropy string (does not need to be a JWT)
- Stored HASHED in the DB (in case of theft, the raw token cannot be 
  read from the DB)
- Usage: When the access token expires, this token is used to obtain a 
  new Access Token (+ optionally a new Refresh Token — rotation)
- Platform-based: Each platform (Web, Mobile) has its own independent 
  session chain (FamilyId) — one does not affect the other (see 4.4)
```

**Possible Table:**

```sql
CREATE TABLE identity_schema.RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Platform NVARCHAR(20) NOT NULL,        -- 'Web' or 'Mobile'
    FamilyId UNIQUEIDENTIFIER NOT NULL,     -- Rotation chain of the same session
    TokenHash NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    ReplacedByTokenId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL
);
```

### 4.3. Refresh Token Rotation (Recommended)

```
On every refresh request:
1. The old refresh token is marked IsRevoked = true and linked to the 
   new token via ReplacedByTokenId
2. A new refresh token is generated and written to the DB (with the 
   same FamilyId)
3. The new Access Token + new Refresh Token are returned together

Benefit: If a stolen refresh token is used once, the real user will 
get an "invalid token" error on their next request — this makes it 
easier to detect suspicious activity (optionally, ALL of the user's 
refresh tokens can be revoked in this case).
```

### 4.4. Platform-Based Session Separation (Web / Mobile)

A user may be logged in on both web and mobile at the same time. These two sessions operate **completely independently** — one does not affect the other's refresh token validity.

```
- User logs in on Web  → a new FamilyId (A) is generated
- User logs in on Mobile → another FamilyId (B) is generated
- The rotation chain (ReplacedByTokenId) only progresses within its 
  own FamilyId
- Rotation happening on Web does not affect FamilyId B (mobile) in any way
```

**Backend endpoint design (how the platform is determined):**

```csharp
[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshRequest? body)
{
    var refreshToken = Request.Cookies["refreshToken"];   // Web: from the cookie
    var platform = ClientPlatform.Web;

    if (string.IsNullOrEmpty(refreshToken) && body != null)
    {
        refreshToken = body.RefreshToken;                  // Mobile: from the body
        platform = ClientPlatform.Mobile;
    }

    var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, platform));

    if (platform == ClientPlatform.Web)
    {
        Response.Cookies.Append("refreshToken", result.NewRefreshToken, cookieOptions);
        return Ok(new { accessToken = result.AccessToken });
    }

    return Ok(new { accessToken = result.AccessToken, refreshToken = result.NewRefreshToken });
}
```

### 4.5. Concurrent Refresh Requests — Server-Side Lock

If multiple refresh requests for the same user+platform arrive at the same time (e.g. two browser tabs, or a concurrent request from the app in the background + foreground), the rotation logic **must not produce an inconsistent result**. This is not left to the client side — it is **resolved on the server side**.

**Approach: Dynamic lock keyed by User+Platform (`IMemoryCache` + `SemaphoreSlim`)**

> Why 2 fixed static objects like `webLock`/`mobileLock` are **not used:** This would unnecessarily queue up ALL users' requests on that platform into a single line (users who have nothing to do with each other would block one another). Instead, a separate, dynamic lock is created for each `UserId + Platform` combination.
>
> Why `IMemoryCache` instead of `ConcurrentDictionary`: So that unused locks (e.g. a user who hasn't made a refresh request in 10 minutes) are automatically cleared from memory — no manual cleanup job needs to be written.
>
> Why DB-level `UPDLOCK` is **not used:** Since the project will run as a single instance (see `architecture.md` Section 6.2), the in-memory lock already provides the same guarantee without adding extra query/transaction overhead to the DB. This decision should be revisited if the project moves to multiple instances.

```csharp
public class RefreshTokenLockProvider
{
    private readonly IMemoryCache _cache;

    public RefreshTokenLockProvider(IMemoryCache cache) => _cache = cache;

    public SemaphoreSlim GetLock(Guid userId, ClientPlatform platform)
    {
        var key = $"refreshlock:{userId}:{platform}";

        return _cache.GetOrCreate(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10); // auto-cleared if unused
            return new SemaphoreSlim(1, 1);
        });
    }
}
```

```csharp
public async Task<RefreshResult> Handle(RefreshTokenCommand request, CancellationToken ct)
{
    var tokenEntity = await _context.RefreshTokens
        .FirstOrDefaultAsync(t => t.TokenHash == request.TokenHash, ct);

    if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        throw new UnauthorizedException("Invalid refresh token");

    // Acquire the lock only for THIS user + THIS platform (does not affect other users)
    var semaphore = _lockProvider.GetLock(tokenEntity.UserId, tokenEntity.Platform);
    await semaphore.WaitAsync(ct);

    try
    {
        // Re-read the current state from the DB AFTER acquiring the lock (source of truth)
        tokenEntity = await _context.RefreshTokens.FirstAsync(t => t.Id == tokenEntity.Id, ct);

        if (tokenEntity.IsRevoked)
            throw new UnauthorizedException("This token has already been used");

        var newRefreshToken = RefreshToken.Create(tokenEntity.UserId, tokenEntity.Platform, tokenEntity.FamilyId);
        tokenEntity.IsRevoked = true;
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.ReplacedByTokenId = newRefreshToken.Id;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(ct);

        var newAccessToken = _tokenService.GenerateAccessToken(tokenEntity.UserId, tokenEntity.Platform);
        return new RefreshResult(newAccessToken, newRefreshToken.RawToken);
    }
    finally
    {
        semaphore.Release(); // MUST always be released via try/finally
    }
}
```

> **Note — Grace Window / Token Family security feature:** An advanced security layer such as a "grace window" (returning the current valid token instead of an error when a revoked token is reused within a short time) can be added for detecting stolen tokens during rotation. This was not deemed necessary at the MVP stage; the lock mechanism above already prevents inconsistency from concurrent requests. It can be considered later if security requirements increase.

---

## 5. Logout / Revoke Scenarios

| Scenario | Behavior |
|----------|----------|
| User logs out | The relevant Refresh Token is set to `IsRevoked = true` |
| Password is changed | ALL of the user's Refresh Tokens are revoked (logout from all devices) |
| Suspicious activity is detected (future) | All Refresh Tokens are revoked |

> Since the Access Token is stateless, even after a refresh token is revoked, the current access token remains valid until it expires (15 min). This is another reason for choosing a short-lived access token — it keeps the security exposure window small.

---

## 6. Authorization Model

- **Role-based authorization** will be used (Customer / Admin).
- On the .NET side, this can be enforced via the `[Authorize(Roles = "Admin")]` attribute or via an **AuthorizationBehavior** at the MediatR Pipeline Behavior level.

```csharp
// Example: role restriction on a Command/Query
public interface IRequireRole
{
    string[] AllowedRoles { get; }
}

public class CreateProductCommand : ICommand<ProductResult>, IRequireRole
{
    public string[] AllowedRoles => new[] { "Admin" };
    // ...
}
```

```csharp
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is IRequireRole requireRole)
        {
            var userRole = _currentUserService.Role; // read from the JWT claim via HttpContext
            if (!requireRole.AllowedRoles.Contains(userRole))
                throw new ForbiddenException("You are not authorized to perform this action.");
        }

        return await next();
    }
}
```

> This uses the same Execute Around logic as Section 5 (Pipeline Behaviors, `architecture.md`) — authorization checks are not manually repeated in every handler; they are guaranteed via a centralized behavior.

---

## 7. Usage Across Modules

- Other modules (Order, Cart, Review, etc.) access the `UserId` and `Role` information via the **Identity module's contract** (e.g. `ICurrentUserService`) whenever they need the user's identity.
- JWT validation is performed at the HTTP Middleware layer (see `architecture.md` Section 7 — Middleware vs Pipeline Behavior distinction); the validated user information is then read by Pipeline Behaviors and Handlers via `ICurrentUserService`.

---

## 8. Open Items / TODO

- [ ] Whether an email verification flow is needed will be clarified
- [ ] The forgot password flow will be detailed
- [ ] Access/Refresh token lifetime values (15 min / 7 days) will be reviewed as the project progresses
- [ ] Login attempt limit / rate limiting (brute-force protection) will be evaluated
- [ ] Grace Window / Token Family stolen-token detection — will be evaluated if security requirements increase
- [ ] If moving to multiple instances: switching from the in-memory lock to a DB-level (UPDLOCK) or distributed lock mechanism will be evaluated
- [ ] Other topics to be discussed will be added here

---

*This document will continue to be updated as the project evolves.*
