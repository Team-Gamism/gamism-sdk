# Gamism SDK

## Communication

Always respond in Korean.

## Project Overview

A modular SDK library for game server (ASP.NET Core) and Unity client. Distributed as NuGet packages and a Unity UPM package.

- **Language**: C#
- **Frameworks**: ASP.NET Core 8, Unity 2021.3+
- **Target Frameworks**: `netstandard2.0` (Core, Unity), `net8.0` (AspNetCore)
- **Package root**: `Gamism.SDK`

## Commands

```bash
# Build all projects
dotnet build

# Build specific project
dotnet build Gamism.SDK.Core/Gamism.SDK.Core.csproj

# Run tests
dotnet test

# Pack NuGet package
dotnet pack Gamism.SDK.Core/Gamism.SDK.Core.csproj -c Release
dotnet pack Gamism.SDK.Extensions.AspNetCore/Gamism.SDK.Extensions.AspNetCore.csproj -c Release
```

## Architecture

### Entry Point (AspNetCore)

`GamismSdkExtensions` is the sole registration entry point:

```csharp
builder.Services.AddGamismSdk(options => { ... });
app.UseGamismSdk();
```

`GamismSdkOptions` composes all feature options. Every feature defaults to `Enabled = true`.

### Projects

| Project | Distribution | Framework | Description |
|---------|-------------|-----------|-------------|
| `Gamism.SDK.Core` | NuGet | netstandard2.0 | Shared types for server and Unity |
| `Gamism.SDK.Unity` | UPM | netstandard2.0 | Unity integration layer |
| `Gamism.SDK.Extensions.AspNetCore` | NuGet | net8.0 | ASP.NET Core feature modules |

### Feature Modules (AspNetCore)

| Feature | Class | Options prefix | Key files |
|---------|-------|----------------|-----------|
| Exception handling | `GlobalExceptionHandler` | `options.Exception` | `Exceptions/GlobalExceptionHandler.cs` |
| Response wrapping | `ApiResponseWrapperFilter` | `options.Response` | `Response/ApiResponseWrapperFilter.cs` |
| Swagger/OpenAPI | `CommonApiResponseOperationFilter` | `options.Swagger` | `Swagger/CommonApiResponseOperationFilter.cs` |
| Request logging | `LoggingFilter` | `options.Logging` | `Logging/LoggingFilter.cs` |

### Key Patterns

**Response wrapping** (`ApiResponseWrapperFilter`): An `IAsyncResultFilter` that wraps all controller responses in `CommonApiResponse<T>` (`{status, code, message, data}`). `null` returns become 204 No Content. Return types implementing `ICommonApiResponse` pass through unchanged with the correct HTTP status code. URL exclusion supports `**` wildcard patterns.

**Exception handling** (`GlobalExceptionHandler`): Handles `ExpectedException` subtypes and maps them to the corresponding HTTP status code. All other exceptions return 500 with a generic message and are logged via `ILogger`.

**Swagger** (`CommonApiResponseOperationFilter`): An `IOperationFilter` that rewrites 200 response schemas to show the `CommonApiResponse` wrapper shape. If the controller return type already implements `ICommonApiResponse`, the `data` field is omitted from the schema.

**Logging** (`LoggingFilter`): An `IMiddleware` that logs `[Request] METHOD /path?query` and `[Response] statusCode (Xms)`. URL exclusion supports `**` wildcard patterns.

**MonoSingleton** (`MonoSingleton<T>`): Unity `MonoBehaviour` singleton. Calls `DontDestroyOnLoad`, auto-destroys duplicate instances, and sets `_isQuitting` on `OnApplicationQuit` to prevent null-reference errors during app shutdown.

**WaitAction**: Callback-based coroutine utilities. `WaitForSeconds` cache uses `int` (milliseconds) as key instead of `float` to avoid floating-point precision issues.

**ApiManager**: Unity HTTP client using `UnityWebRequest`. Always returns `CommonApiResponse<T>` — network errors are also returned as `CommonApiResponse.Error<T>` rather than thrown.

### Core Shared Types

| Type | Description |
|------|-------------|
| `CommonApiResponse<T>` | Shared response format `{status, code, message, data}` |
| `ICommonApiResponse` | Marker interface used by `ApiResponseWrapperFilter` and Swagger filter |
| `ExpectedException` | Base exception carrying `HttpStatusCode`. Stack trace is not logged by `GlobalExceptionHandler` |
| `IJsonSerializer` | JSON serialization abstraction (Unity: `NewtonsoftJsonSerializer`) |
| `SingletonBase<T>` | Thread-safe singleton for pure C# environments |

## Commit Convention

- `feat:` new feature
- `update:` modification
- `docs:` documentation
- `fix:` bug fix
