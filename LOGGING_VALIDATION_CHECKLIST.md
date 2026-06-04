# Logging System Validation Checklist

## Pre-Deployment Verification

### ✅ Installation & Setup

- [ ] Serilog NuGet packages installed
  ```bash
  dotnet restore
  ```

- [ ] No compilation errors
  ```bash
  dotnet build
  ```

- [ ] Program.cs has Serilog configuration
  - [ ] `builder.Host.UseSerilog()`
  - [ ] Console output configured
  - [ ] File output configured
  - [ ] Enrich settings configured

- [ ] Middleware registered
  - [ ] `app.UseMiddleware<ExceptionHandlingMiddleware>()`
  - [ ] `app.UseMiddleware<RequestLoggingMiddleware>()`

- [ ] DbContext logging enabled
  - [ ] `.LogTo(Console.WriteLine, LogLevel.Debug)`
  - [ ] `.EnableSensitiveDataLogging()` (dev only)

### ✅ Controller Setup

- [ ] ILogger<T> injected in UnifiedDocumentController
- [ ] Constructor accepts logger parameter
- [ ] GetByVin method has:
  - [ ] RequestId tracking
  - [ ] Input validation logging
  - [ ] Parameter parsing logging
  - [ ] Service call logging
  - [ ] Error handling with categorization

### ✅ Service Setup

- [ ] UnifiedDocumentService has ILogger<T>
  - [ ] OperationId generated for each call
  - [ ] Sale API call logged
  - [ ] Service API call logged
  - [ ] Error collection and aggregation logged

- [ ] SaleAPI has ILogger<T>
  - [ ] Constructor accepts logger
  - [ ] Database queries logged
  - [ ] Data mapping progress logged
  - [ ] Result construction logged
  - [ ] Exception categorization (DbUpdateException, etc.)

- [ ] ServiceAPI has ILogger<T>
  - [ ] Constructor accepts logger
  - [ ] Database queries logged
  - [ ] Data mapping progress logged
  - [ ] Result construction logged
  - [ ] Exception categorization

### ✅ Repository Setup

- [ ] GenericRepository has ILogger<T>
  - [ ] Constructor accepts logger
  - [ ] AddAsync method logs
  - [ ] SaveChangesAsync method logs with change count
  - [ ] DeleteAsync method logs with found/not found status
  - [ ] GetAllAsync method logs with record count
  - [ ] GetByIdAsync method logs with found/not found status
  - [ ] UpdateAsync method logs
  - [ ] All methods have OperationId tracking
  - [ ] Exception handling for DbUpdateException and DbUpdateConcurrencyException

### ✅ Middleware Setup

- [ ] ExceptionHandlingMiddleware
  - [ ] Catches all exception types
  - [ ] Categorizes exceptions (9 types)
  - [ ] Logs exception details
  - [ ] Returns appropriate HTTP status codes
  - [ ] Returns structured APIResponse

- [ ] RequestLoggingMiddleware
  - [ ] Logs request initiation
  - [ ] Captures request details (method, path, IP)
  - [ ] Measures request duration
  - [ ] Logs response completion
  - [ ] Handles exceptions within middleware

---

## Functional Testing

### ✅ Happy Path Testing

**Test 1: Successful Request**
- [ ] Make request: `GET /api/UnifiedDocument/12345`
- [ ] Verify HTTP 200 response
- [ ] Check logs contain:
  - [ ] "GetByVin endpoint called"
  - [ ] "Starting GetDocumentsByVinAsync"
  - [ ] "Fetching sale data"
  - [ ] "Fetching service data"
  - [ ] "Successfully retrieved documents"
  - [ ] "HTTP response completed"
  - [ ] Duration measurement > 0

**Test 2: Data Not Found**
- [ ] Make request: `GET /api/UnifiedDocument/99999` (non-existent VIN)
- [ ] Verify HTTP 404 response
- [ ] Check logs contain:
  - [ ] "No sales transaction found" OR "No documents found"
  - [ ] "HTTP response completed"

**Test 3: Multiple API Failures**
- [ ] Temporarily disable/block SaleAPI (simulate failure)
- [ ] Make request: `GET /api/UnifiedDocument/12345`
- [ ] Check logs contain:
  - [ ] "Sale API error" or exception message
  - [ ] "Service API" logs still present (parallel execution)
  - [ ] Service handles partial failure gracefully

### ✅ Error Path Testing

**Test 4: Invalid Input**
- [ ] Make request: `GET /api/UnifiedDocument/` (no VIN)
- [ ] Verify HTTP 400 or 404 response
- [ ] Check logs contain:
  - [ ] "VIN is required" or validation error

**Test 5: Malformed Parameter**
- [ ] Make request: `GET /api/UnifiedDocument/ABC123` (non-numeric)
- [ ] Check logs contain:
  - [ ] "Failed to parse VIN to integer"
  - [ ] HTTP 400 Bad Request response

**Test 6: Database Connection Error**
- [ ] Temporarily disable database connection
- [ ] Make request: `GET /api/UnifiedDocument/12345`
- [ ] Check logs contain:
  - [ ] "DbUpdateException" OR "Database error"
  - [ ] Full exception stack trace
  - [ ] HTTP 500 or 502 response

**Test 7: External API Timeout**
- [ ] Configure short timeout in DbContext/HTTP client
- [ ] Make request: `GET /api/UnifiedDocument/12345`
- [ ] Check logs contain:
  - [ ] "TaskCanceledException" OR "timeout"
  - [ ] "Request timeout" message
  - [ ] HTTP 504 response

**Test 8: Concurrent Requests**
- [ ] Send multiple requests simultaneously
- [ ] Verify each has unique RequestId
- [ ] Verify logs can be correlated to individual requests
- [ ] No log entries mixed between requests

---

## Performance Validation

### ✅ Response Time Checks

- [ ] Normal request completes in < 2 seconds
- [ ] Failed query logs within < 1 second
- [ ] Database query logs appear in logs
- [ ] Response time logged in milliseconds
- [ ] Duration calculation accurate

### ✅ Log File Checks

- [ ] Logs directory created
- [ ] Log files rolling daily
- [ ] Log file naming correct: `unified-document-viewer-YYYY-MM-DD.txt`
- [ ] Log size limited (< 10MB per file)
- [ ] Oldest files auto-deleted after 7 days

### ✅ Log Volume Checks

- [ ] Average log lines per request: 10-30 (acceptable)
- [ ] Storage: ~1MB per day (estimated)
- [ ] No duplicate logs
- [ ] No empty logs

---

## Log Content Validation

### ✅ Log Message Quality

- [ ] All logs have timestamps
- [ ] All logs have severity level [INF], [WRN], [ERR], [DBG]
- [ ] All structured properties formatted correctly
- [ ] RequestId appears in request flow logs
- [ ] OperationId appears in service/repository logs
- [ ] No sensitive data logged (passwords, tokens, etc.)
- [ ] Exception stack traces complete and readable

### ✅ Specific Message Checks

- [ ] "[INF] HTTP GET request initiated" appears once per request
- [ ] "[INF] GetByVin endpoint called with VIN" appears with VIN value
- [ ] "[DBG] Querying SalesTransactions table" appears for DB queries
- [ ] "[INF] HTTP response completed" includes StatusCode and Duration
- [ ] "[ERR]" or "[WRN]" messages include exception type and message

### ✅ Correlation Checks

- [ ] RequestId consistent throughout request
- [ ] OperationId unique per operation
- [ ] All related logs grouped (same RequestId together)
- [ ] Timestamps show reasonable progression

---

## Search & Analysis Validation

### ✅ Grep/Search Tests

```bash
# Test 1: Find by RequestId
grep "0HN5K4F1D5GJR:00000001" logs/*.txt
# Expected: Multiple entries with same RequestId

# Test 2: Find by VIN
grep "VIN: 12345" logs/*.txt
# Expected: Multiple entries showing VIN processing

# Test 3: Find errors
grep -i "\[ERR\]" logs/*.txt
# Expected: Only error-level logs

# Test 4: Find duration times
grep "Duration:" logs/*.txt
# Expected: All responses show duration
```

- [ ] RequestId search returns complete request flow
- [ ] VIN search returns all related operations
- [ ] Error search returns only error-level logs
- [ ] Duration search returns all completed requests
- [ ] OperationId search returns operation-specific logs

---

## Security Validation

### ✅ Development Mode

- [ ] Sensitive data logging enabled (intended)
- [ ] SQL queries visible in logs
- [ ] Parameter values shown
- [ ] Stack traces complete

### ✅ Production Preparation

- [ ] Sensitive data logging disabled in production config
- [ ] Min log level set to Warning or higher
- [ ] Log file permissions configured (read-only for most users)
- [ ] Rotation and retention policy in place
- [ ] No credentials in connection string logs

### ✅ Data Protection

- [ ] Passwords not logged
- [ ] API tokens not logged
- [ ] Database credentials partially masked/not shown
- [ ] User PII handled carefully (not logged unless necessary)

---

## Documentation Validation

### ✅ Guides Present

- [ ] LOGGING_IMPLEMENTATION.md exists and complete
- [ ] LOGGING_QUICK_REFERENCE.md exists and usable
- [ ] LOGGING_SUMMARY.md exists
- [ ] LOGGING_FLOW_DIAGRAMS.md exists

### ✅ Guide Contents

- [ ] Implementation guide has architecture explanation
- [ ] Quick reference has code examples
- [ ] Flow diagrams show request flow
- [ ] Troubleshooting guides cover 6+ scenarios
- [ ] Search patterns documented
- [ ] Performance monitoring thresholds documented

---

## Integration Testing

### ✅ Full Stack Test

```
Setup:
1. Start application
2. Make real API request
3. Verify response
4. Check log file
5. Verify all layers logged
```

- [ ] Application starts without errors
- [ ] API endpoint responds correctly
- [ ] Log file created in logs/ directory
- [ ] Log file has correct naming
- [ ] All log entries present:
  - [ ] RequestLoggingMiddleware start
  - [ ] Controller logging
  - [ ] Service logging
  - [ ] Repository logging
  - [ ] RequestLoggingMiddleware end

### ✅ Error Scenario Integration

```
Setup:
1. Simulate error condition
2. Make API request
3. Verify error response
4. Check error logging
5. Verify exception categorization
```

- [ ] ExceptionHandlingMiddleware logs exception
- [ ] Correct HTTP status code returned
- [ ] Error message in response
- [ ] Stack trace in log file
- [ ] Exception categorized correctly

---

## Deployment Readiness

### ✅ Pre-Production Checklist

- [ ] All NuGet packages installed correctly
- [ ] No compilation warnings
- [ ] Logs directory path verified
- [ ] File permissions checked
- [ ] Disk space adequate (7 days × 1MB ~7MB)
- [ ] Log rotation verified
- [ ] Sensitive data logging disabled
- [ ] Min log level appropriate
- [ ] Correlation IDs working
- [ ] Performance within thresholds

### ✅ Production Checklist

- [ ] Logs secure (restricted access)
- [ ] Monitoring/alerting configured
- [ ] Log aggregation setup (if applicable)
- [ ] Backup strategy for logs
- [ ] Performance baseline established
- [ ] Error threshold alerts configured
- [ ] On-call team trained on log analysis
- [ ] Documentation accessible to team

---

## Quick Validation Commands

```bash
# Verify installation
dotnet restore
dotnet build

# Start application
dotnet watch run

# Monitor logs in real-time
tail -f logs/unified-document-viewer-*.txt

# Test endpoint
curl http://localhost:5000/api/UnifiedDocument/12345

# Count log entries
wc -l logs/unified-document-viewer-*.txt

# Find errors
grep -i "error\|exception" logs/unified-document-viewer-*.txt | head -10

# Check log file age
ls -la logs/

# Verify RequestId in logs
grep "RequestId" logs/unified-document-viewer-*.txt | head -1
```

---

## Success Criteria

✅ **PASS** if ALL of the following are true:

1. No compilation errors or warnings
2. Application starts successfully
3. API endpoints respond correctly
4. Logs are written to file
5. All log layers appear (controller, service, repository)
6. Errors are properly categorized
7. RequestId/OperationId tracking works
8. Performance within acceptable range
9. No sensitive data exposed in logs
10. Documentation is complete and accurate

❌ **FAIL** if ANY of the following are true:

1. Compilation errors
2. Application crashes on startup
3. Logs not being written
4. Missing log entries at any layer
5. Incorrect status codes on errors
6. Sensitive data in logs
7. Performance significantly degraded
8. Correlation IDs not working
9. Log file rotation not working
10. Documentation incomplete

