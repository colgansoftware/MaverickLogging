# Maverick.Logging

A standardized, production-ready logging package for .NET applications built on **Microsoft.Extensions.Logging** and **Serilog**.

---

## Overview

Maverick.Logging provides a consistent logging implementation across all applications with:

- Structured logging via Serilog
- Rolling file logs
- Dedicated error logs
- Domain-specific logs (IMAP, SECURITY)
- Runtime log level switching (no restart required)
- Clean integration with `ILogger<T>`

---

## Installation

### From Local NuGet Source

```bash
dotnet add package Maverick.Logging --source LocalNuget
```

Or via Visual Studio:

1. Go to **Tools → NuGet Package Manager → Package Manager Settings**
2. Add a new source:
   - Name: `LocalNuget`
   - Source: `C:\Maverick\Nuget` (or your configured path)
3. Install `Maverick.Logging`

---

## Quick Start (Required Setup)

In `Program.cs`:

```csharp
using Maverick.Logging;

var builder = WebApplication.CreateBuilder(args);

var logRoot = Path.Combine(
    builder.Environment.ContentRootPath,
    "Logs");

builder.Logging.AddMaverickLogging(
    builder.Environment,
    builder.Configuration,
    logRoot,
    builder.Environment.ApplicationName);
```

---

## Output Structure

Logs are written to:

```
<ContentRoot>\Logs\
```

Files generated:

| File | Purpose |
|------|--------|
| `AppName-YYYYMMDD.log` | All log events |
| `Error-YYYYMMDD.log` | Errors only |
| `IMAP-YYYYMMDD.log` | IMAP-specific events |
| `Security-YYYYMMDD.log` | Security events |

---

## Logging Usage

### Standard Logging

```csharp
logger.LogInformation("Application started");
```

### Error Logging

```csharp
logger.LogError(ex, "Unhandled exception occurred");
```

### IMAP Logging (Custom Channel)

```csharp
logger
    .ForContext("IMAP", true)
    .Information("Connected to mailbox");
```

### Security Logging (Custom Channel)

```csharp
logger
    .ForContext("SECURITY", true)
    .Warning("Unauthorized access attempt");
```

---

## Log Levels by Environment

| Environment | Default Level |
|------------|--------------|
| Development | Debug |
| Staging | Information |
| Production | Warning |

---

## Dynamic Log Level Changes

You can change logging levels at runtime via `appsettings.json`.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

> Requires `reloadOnChange: true` in configuration.

---

## Filtering Behavior

The following noisy categories are suppressed by default:

- Microsoft → Warning+
- System → Warning+
- Microsoft.Hosting.Lifetime → Disabled
- Kestrel → Disabled

---

## Developer Requirements

To use this package correctly:

1. Always inject `ILogger<T>`
2. Use structured logging (avoid string concatenation)
3. Use context properties for domain-specific logs (IMAP, SECURITY)

---

## Example

```csharp
public class EmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public void Connect()
    {
        _logger.ForContext("IMAP", true)
               .Information("Connecting to IMAP server");
    }
}
```

---

## Versioning Strategy

- Patch: Bug fixes
- Minor: Backward-compatible enhancements
- Major: Breaking changes

---

## Recommended Practices

- Do not log sensitive data (passwords, tokens)
- Use `LogError` for exceptions (always include `ex`)
- Prefer structured logging:

```csharp
logger.LogInformation("User {UserId} logged in", userId);
```

---

## Troubleshooting

### Logs not appearing

- Verify `logRoot` path exists
- Ensure app has write permissions
- Confirm package is installed

### Log level not updating

- Ensure `reloadOnChange = true`
- Verify correct JSON path: `Serilog:MinimumLevel:Default`

---

## Future Enhancements (Planned)

- Correlation ID middleware
- Distributed tracing support
- Optional sinks (Seq, Elastic, App Insights)

---

## Ownership

Maintained by Maverick development team.

For changes, update the package and increment version in `.csproj`.

