# Quick Logging Reference & Usage Guide

## How to Add Logging to New Code

### 1. **In Services/Controllers**

```csharp
private readonly ILogger<YourClassName> _logger;

public YourClassName(ILogger<YourClassName> logger)
{
    _logger = logger;
}

public async Task YourMethod(int id)
{
    var operationId = Guid.NewGuid().ToString("N")[..8];
    using (LogContext.PushProperty("OperationId", operationId))
    {
        _logger.LogInformation("Starting YourMethod with id: {Id} | OperationId: {OperationId}", id, operationId);
        
        try
        {
            // Your code here
            _logger.LogDebug("Processing step 1 for id: {Id}", id);
            
            // Operation success
            _logger.LogInformation("YourMethod completed successfully for id: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in YourMethod for id: {Id} | Error: {ErrorMessage}", id, ex.Message);
            throw;
        }
    }
}
```

### 2. **In Repositories**

```csharp
public virtual async Task<T?> GetByIdAsync(int id)
{
    var operationId = Guid.NewGuid().ToString("N")[..8];
    using (LogContext.PushProperty("OperationId", operationId))
    {
        _logger.LogInformation("GetByIdAsync for {EntityType} with id: {Id}", typeof(T).Name, id);
        
        try
        {
            var entity = await _context.Set<T>().FindAsync(id);
            
            if (entity != null)
            {
                _logger.LogDebug("Entity found: {EntityType} with id: {Id}", typeof(T).Name, id);
            }
            else
            {
                _logger.LogWarning("Entity not found: {EntityType} with id: {Id}", typeof(T).Name, id);
            }
            
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} with id: {Id}", typeof(T).Name, id);
            throw;
        }
    }
}
```

---

## Log Level Guidelines

### 📊 When to Use Each Level

| Level | Usage | Example |
|-------|-------|---------|
| **Critical** | System failure, restart required | Database server down |
| **Error** | Operation failed, but system continues | Failed database query, API unreachable |
| **Warning** | Unexpected situation, may cause issues | Data not found, slow query detected |
| **Information** | General flow, significant events | Request received, operation completed |
| **Debug** | Detailed diagnostic info | Query parameters, intermediate steps |
| **Trace** | Very detailed, rarely used | Variable state at each line |

### 📋 Practical Examples

```csharp
// CRITICAL - System down
_logger.LogCritical("Database connection pool exhausted - cannot proceed");

// ERROR - Operation failed
_logger.LogError(ex, "Failed to save customer {CustomerId}: {Message}", customerId, ex.Message);

// WARNING - Unexpected but handled
_logger.LogWarning("No warranty found for VIN {Vin}, returning null", vin);

// INFORMATION - Important events
_logger.LogInformation("Successfully processed {Count} transactions", transactionCount);

// DEBUG - Development/diagnosis
_logger.LogDebug("Query parameters: VIN={Vin}, StartDate={StartDate}", vin, startDate);
```

---

## Structured Logging Best Practices

### ✅ Good Examples

```csharp
// Use named properties for filtering/searching
_logger.LogInformation("Order processed | OrderId: {OrderId} | CustomerId: {CustomerId} | Amount: {Amount}",
    orderId, customerId, amount);

// Include context and parameters
_logger.LogError(ex, "Failed to fetch sale data for VIN: {Vin} | RequestId: {RequestId}",
    vin, requestId);

// Log both success and failure paths
if (result != null)
{
    _logger.LogInformation("Successfully retrieved {RecordCount} records", result.Count);
}
else
{
    _logger.LogWarning("No records found for query: {Query}", query);
}
```

### ❌ Avoid

```csharp
// Don't use string concatenation
_logger.LogInformation("Order: " + orderId + " Amount: " + amount); // Bad!

// Don't log sensitive data in Production
_logger.LogDebug("Password: {Password}"); // Bad in production!

// Don't use generic messages
_logger.LogError("Error occurred"); // Too vague!

// Don't log at wrong levels
_logger.LogError("User requested data"); // Should be Information!
```

---

## Filtering Logs in Log Files

### Find Errors
```bash
# Linux/Mac
grep "ERROR\|FATAL" logs/*.txt

# Windows PowerShell
Select-String -Path "logs/*.txt" -Pattern "ERROR|FATAL"
```

### Find Specific VIN
```bash
grep "VIN: 12345" logs/*.txt
```

### Find Slow Operations
```bash
grep "Duration: [5-9][0-9][0-9][0-9]" logs/*.txt  # > 5000ms
```

### Find Request Trace
```bash
grep "0HN5K4F1D5GJR:00000001" logs/*.txt  # Use your RequestId
```

---

## Common Debugging Scenarios

### Scenario 1: "Data not being returned for VIN 12345"

```
Search pattern: "VIN: 12345"
Expected logs:
  1. "Starting GetDocumentsByVinAsync for VIN: 12345" → Service called
  2. "Fetching sale data from Sale API for VIN: 12345" → Sale API called
  3. "Found sales transaction for VIN: 12345" OR "No sales transaction found" → Result
  4. "Fetching service data from Service API for VIN: 12345" → Service API called
  5. "Found service record for VIN: 12345" OR "No service record found" → Result
  
If step 3 and 5 show "not found":
  → Data doesn't exist in databases, not a bug
  
If logs are missing:
  → Request didn't reach the service, check controller logs
  
If exception logs exist:
  → Database/API error, check exception message for details
```

### Scenario 2: "Request timing out (taking > 2 seconds)"

```
Search pattern: "HTTP response completed"
Look for: "Duration: XXXX"
   If > 2000ms → Slow operation
   
Check individual component logs:
  - SaleAPI logs (search: "Starting GetSaleDataByVinAsync")
  - ServiceAPI logs (search: "Starting GetServiceDataByVinAsync")
  - Repository logs (search: "GetByIdAsync")
  
Whichever component has the highest duration is the bottleneck
```

### Scenario 3: "API returning 502 Bad Gateway"

```
Search logs for exception containing:
  - "HttpRequestException" → Network/API issue
  - "TaskCanceledException" → API timeout
  - Check which API: "Sale API" or "Service API"
  
In ExceptionHandlingMiddleware logs:
  - Status code should be 502
  - Check error details in response
  
Next steps:
  1. Check external API availability
  2. Check network connectivity
  3. Check timeout settings
  4. Review external API logs
```

### Scenario 4: "Database errors"

```
Search pattern: "DbUpdateException"
Look for:
  - Which entity type: Customer, SalesTransaction, ServiceRecord, etc.
  - Which operation: AddAsync, UpdateAsync, SaveChangesAsync
  - Exception message contains SQL error details
  
Common causes:
  - Constraint violations (FK, unique key)
  - Data type mismatches
  - Connection issues
  
Check database logs and constraints
```

---

## Performance Monitoring via Logs

### Create Performance Summary

```csharp
// Sample analysis script (can be automated)
var logs = File.ReadAllLines("logs/unified-document-viewer-2024-01-15.txt");

var slowRequests = logs
    .Where(l => l.Contains("Duration:"))
    .Where(l => int.Parse(l.Split("Duration: ")[1].Split("ms")[0]) > 1000)
    .ToList();

Console.WriteLine($"Slow requests (>1000ms): {slowRequests.Count}");
foreach (var req in slowRequests.Take(5))
{
    Console.WriteLine(req);
}
```

### Key Metrics to Monitor

1. **Average Response Time**
   - Should be < 500ms for typical requests
   - > 2000ms indicates performance issue

2. **Error Rate**
   - Count ERROR logs / Count of all requests
   - Should be < 1%

3. **Database Query Time**
   - Look for SaveChangesAsync duration
   - Should be < 200ms per operation

4. **API Call Duration**
   - Sale API and Service API calls
   - Should complete within 5 seconds

---

## Integration with External Monitoring

### For ELK Stack
```
Parse structured logs using:
- Timestamp: First field
- Level: Enclosed in []
- Message: Everything after Level
- OperationId/RequestId: Extracted from message
```

### For Application Insights
```csharp
// Add to Serilog configuration:
.WriteTo.ApplicationInsights(new TelemetryClient(), TelemetryConverter.Traces)
```

### For Splunk
```
Serilog configuration to send to Splunk HEC:
.WriteTo.Splunk(new HecOnboardingToken("..."))
```

---

## Troubleshooting Logging Issues

### Logs not being written to file

```
Check:
1. logs/ directory exists with write permissions
2. Serilog NuGet packages are installed
3. No exceptions in application startup logs
4. File isn't locked by another process
```

### Logs missing important information

```
Verify:
1. _logger injection is not null
2. Logging level includes your log calls
3. Use proper placeholders {PropertyName}
4. Don't use string concatenation
```

### Performance impact from logging

```
Recommendations:
1. Use async file sink
2. Adjust batch size in rolling file sink
3. Use appropriate logging levels (not all Debug in Production)
4. Filter out noisy logs
```

