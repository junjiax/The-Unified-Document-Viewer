# Comprehensive Logging Implementation Guide

## Overview
This document describes the comprehensive logging system implemented throughout the Unified Document Viewer application to facilitate debugging, monitoring, and malfunction inspection.

## Logging Architecture

### 1. **Serilog Configuration** (Program.cs)
The application uses **Serilog** for structured logging with the following features:

```
Configuration Level: Debug
Output Destinations:
  - Console: Real-time log output during development
  - File: Rolling daily files in logs/ directory (retention: 7 days, max 10MB each)
```

**Log Template:**
```
[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}
```

**Enrichment:**
- Request ID tracking
- Application name context
- Operation IDs for distributed tracing

---

## Logging Layers & Components

### 2. **Middleware Layer**

#### A. **ExceptionHandlingMiddleware** (Middleware/ExceptionHandlingMiddleware.cs)
**Purpose:** Global exception handler with structured error responses

**Key Logging Points:**
- ✅ Request initialization (Method, Path, Remote IP)
- ✅ Request completion with status code
- ✅ Exception capture with full stack trace and categorization
  - `ArgumentNullException` → 400 Bad Request
  - `ArgumentException` → 400 Bad Request
  - `InvalidOperationException` → 409 Conflict
  - `TimeoutException` → 504 Gateway Timeout
  - `HttpRequestException` → 502 Bad Gateway
  - Generic exceptions → 500 Internal Server Error

**Example Log Output:**
```
[2024-01-15 10:30:45.123 +05:00] [ERR] Unhandled exception occurred during request processing for GET /api/UnifiedDocument/ABC123
```

#### B. **RequestLoggingMiddleware** (Middleware/RequestLoggingMiddleware.cs)
**Purpose:** Request/response lifecycle tracking

**Key Logging Points:**
- ✅ HTTP request initiation (Method, Path, Query String, ClientIP, RequestId)
- ✅ Request duration measurement (milliseconds)
- ✅ Response status code
- ✅ User context (authenticated user or "Anonymous")
- ✅ Request ID propagation for correlation

**Example Log Output:**
```
[2024-01-15 10:30:45.100 +05:00] [INF] HTTP GET request initiated: /api/UnifiedDocument/ABC123 | ClientIP: 192.168.1.100 | RequestId: 0HN5K4F1D5GJR:00000001
[2024-01-15 10:30:45.850 +05:00] [INF] HTTP response completed: GET /api/UnifiedDocument/ABC123 | StatusCode: 200 | Duration: 750ms | RequestId: 0HN5K4F1D5GJR:00000001
```

---

### 3. **Controller Layer**

#### **UnifiedDocumentController** (Controllers/UnifiedDocument/UnifiedDocumentController.cs)
**Purpose:** API endpoint logging and request validation

**Key Logging Points:**
- ✅ Endpoint invocation with parameters
- ✅ Input validation (null/empty VIN checks)
- ✅ VIN parsing attempts and failures
- ✅ Service call initiation
- ✅ Document retrieval success/failure
- ✅ Exception handling for different error types:
  - HTTP errors (external API failures)
  - Timeout errors
  - General exceptions

**Logging Levels:**
- `Information` - Request start, successful operations
- `Warning` - Input validation failures, no data found
- `Debug` - Parsing operations
- `Error` - Exceptions and system failures

**Example Log Output:**
```
[2024-01-15 10:30:45.125 +05:00] [INF] GetByVin endpoint called with VIN: 12345 | RequestId: 0HN5K4F1D5GJR:00000001
[2024-01-15 10:30:45.130 +05:00] [DBG] Attempting to parse VIN: 12345 to integer | RequestId: 0HN5K4F1D5GJR:00000001
[2024-01-15 10:30:45.135 +05:00] [INF] Starting document retrieval for VIN: 12345 | RequestId: 0HN5K4F1D5GJR:00000001
[2024-01-15 10:30:45.850 +05:00] [INF] Successfully retrieved documents for VIN: 12345 | RequestId: 0HN5K4F1D5GJR:00000001
```

---

### 4. **Service Layer**

#### A. **UnifiedDocumentService** (Services/UnifiedDocumentService/UnifiedDocumentService.cs)
**Purpose:** Orchestration of Sale and Service API calls with error aggregation

**Key Logging Points:**
- ✅ Service method invocation
- ✅ Sale API call initiation and results
- ✅ Service API call initiation and results
- ✅ Individual error capture for each API (with error categorization)
- ✅ Summary logging with error count
- ✅ Final result composition

**Error Categorization:**
- `HttpRequestException` - API connectivity issues
- `TaskCanceledException` - Request timeout
- Generic `Exception` - Unexpected errors

**Example Log Output:**
```
[2024-01-15 10:30:45.140 +05:00] [INF] Starting GetDocumentsByVinAsync for VIN: 12345 | OperationId: a1b2c3d4
[2024-01-15 10:30:45.150 +05:00] [INF] Fetching sale data from Sale API for VIN: 12345
[2024-01-15 10:30:45.450 +05:00] [INF] Successfully retrieved sale data for VIN: 12345
[2024-01-15 10:30:45.460 +05:00] [INF] Fetching service data from Service API for VIN: 12345
[2024-01-15 10:30:45.550 +05:00] [INF] Successfully retrieved service data for VIN: 12345
[2024-01-15 10:30:45.560 +05:00] [INF] GetDocumentsByVinAsync completed successfully for VIN: 12345 | Has Sale Data: True, Has Service Data: True
```

#### B. **SaleAPI** (Services/UnifiedDocumentService/SaleAPI.cs)
**Purpose:** Sale database queries and data transformation

**Key Logging Points:**
- ✅ Query initiation with VIN parameter
- ✅ Sale transaction retrieval (TransactionId, CustomerId)
- ✅ Sales document retrieval (DocumentId, Type)
- ✅ Warranty registration retrieval (WarrantyId, Type)
- ✅ Data mapping progress
- ✅ Result composition success
- ✅ Exception handling:
  - `DbUpdateException` - Database errors
  - `InvalidOperationException` - Query errors
  - Generic exceptions

**Example Log Output:**
```
[2024-01-15 10:30:45.150 +05:00] [INF] Starting GetSaleDataByVinAsync for VIN: 12345 | OperationId: a1b2c3d4
[2024-01-15 10:30:45.155 +05:00] [DBG] Querying SalesTransactions table for VIN: 12345
[2024-01-15 10:30:45.250 +05:00] [INF] Found sales transaction for VIN: 12345 | TransactionId: 98765 | CustomerId: 100
[2024-01-15 10:30:45.255 +05:00] [DBG] Querying SalesDocuments table for VIN: 12345
[2024-01-15 10:30:45.300 +05:00] [DBG] Found sales document for VIN: 12345 | DocumentId: 5001 | Type: SalesInvoice
[2024-01-15 10:30:45.350 +05:00] [INF] Successfully built sale data for VIN: 12345 | Has Customer: True, Has FinancingContract: True
```

#### C. **ServiceAPI** (Services/UnifiedDocumentService/ServiceAPI.cs)
**Purpose:** Service database queries and data transformation

**Key Logging Points:**
- ✅ Query initiation with VIN parameter
- ✅ Service record retrieval (ServiceRecordId, Type, Status)
- ✅ Service document retrieval (DocumentId, Type)
- ✅ Diagnostic report retrieval (ReportId, Severity, Code)
- ✅ Technician assignment tracking
- ✅ Data mapping progress
- ✅ Result composition success
- ✅ Exception handling (same categories as SaleAPI)

**Example Log Output:**
```
[2024-01-15 10:30:45.460 +05:00] [INF] Starting GetServiceDataByVinAsync for VIN: 12345 | OperationId: e5f6g7h8
[2024-01-15 10:30:45.465 +05:00] [DBG] Querying ServiceRecords table for VIN: 12345
[2024-01-15 10:30:45.500 +05:00] [INF] Found service record for VIN: 12345 | ServiceRecordId: 450 | ServiceType: Maintenance | Status: Completed
[2024-01-15 10:30:45.505 +05:00] [DBG] Technician assigned to VIN: 12345 | TechnicianId: 25 | Name: John Smith
[2024-01-15 10:30:45.540 +05:00] [INF] Successfully built service data for VIN: 12345 | Has Technician: True, Has DiagnosticReport: False
```

---

### 5. **Repository Layer**

#### **GenericRepository<T>** (Repositories/GenericRepository.cs)
**Purpose:** Database operation tracking

**Key Logging Points:**

1. **AddAsync** - Entity addition
   - ✅ Entity type being added
   - ✅ Addition to context
   - ✅ Exceptions: DbUpdateException

2. **SaveChangesAsync** - Persistence operations
   - ✅ Changes initiated
   - ✅ Number of changes saved
   - ✅ Exceptions: DbUpdateException, DbUpdateConcurrencyException

3. **DeleteAsync** - Entity deletion
   - ✅ Entity type and ID
   - ✅ Entity found/not found status
   - ✅ Marking for deletion

4. **GetAllAsync** - Retrieve all records
   - ✅ Entity type requested
   - ✅ Record count returned
   - ✅ Generic exceptions

5. **GetByIdAsync** - Retrieve by ID
   - ✅ Entity type and ID
   - ✅ Found/not found status
   - ✅ Generic exceptions

6. **UpdateAsync** - Entity modification
   - ✅ Entity type being updated
   - ✅ Update initiation
   - ✅ Generic exceptions

**Example Log Output:**
```
[2024-01-15 10:30:45.300 +05:00] [INF] GetByIdAsync called for entity type: Customer with id: 100 | OperationId: xyz123
[2024-01-15 10:30:45.305 +05:00] [DBG] Entity found: Customer with id: 100 | OperationId: xyz123
[2024-01-15 10:30:45.350 +05:00] [INF] AddAsync called for entity type: SalesTransaction | OperationId: xyz124
[2024-01-15 10:30:45.355 +05:00] [INF] SaveChangesAsync called | OperationId: xyz125
[2024-01-15 10:30:45.400 +05:00] [INF] SaveChangesAsync completed successfully. Changes saved: 1 | OperationId: xyz125
```

---

## Log File Structure

**Location:** `logs/` directory in the application root

**File Format:** `unified-document-viewer-YYYY-MM-DD.txt`

**Example:**
```
logs/
├── unified-document-viewer-2024-01-15.txt (current day)
├── unified-document-viewer-2024-01-14.txt
├── unified-document-viewer-2024-01-13.txt
└── ... (up to 7 days retention)
```

---

## Troubleshooting Guide

### Finding Issues in Logs

**1. API Request Failed (500 Error)**
```
Search for: "ERROR" + "GET /api/UnifiedDocument"
Look for: Exception type and message in ExceptionHandlingMiddleware logs
```

**2. Database Connection Issues**
```
Search for: "Database error" or "DbUpdateException"
Check: SaleAPI/ServiceAPI logs with VIN number
Verify: Connection strings in appsettings.json
```

**3. Timeout Issues**
```
Search for: "TaskCanceledException" or "timeout"
Check: Duration times > 30000ms (30 seconds)
Consider: Increasing database/API timeout settings
```

**4. Data Not Found**
```
Search for: "No [sale/service] data found" or "not found"
Check: VIN parameter is correct
Verify: Data exists in corresponding database tables
```

**5. External API Issues**
```
Search for: "HttpRequestException" or "External service error"
Check: External API endpoints (Sale API, Service API)
Monitor: Network connectivity and API health
```

**6. Performance Issues**
```
Search for: "Duration:" in response logs
Calculate: Average response times across requests
Identify: Queries taking > 1000ms
Optimize: Slow database queries or API calls
```

---

## Correlation IDs

The system uses **RequestId** and **OperationId** for request tracing:

- **RequestId:** Unique identifier for each HTTP request (auto-generated by ASP.NET Core)
- **OperationId:** Unique identifier for service-level operations (generated within services)

**Example Trace:**
```
HTTP Request: 0HN5K4F1D5GJR:00000001
├── UnifiedDocumentService OperationId: a1b2c3d4
│   ├── SaleAPI OperationId: x1y2z3a4
│   └── ServiceAPI OperationId: p1q2r3s4
└── GenericRepository operations with their own OperationIds
```

**To track a complete flow:**
1. Find the HTTP RequestId in response logs
2. Search logs for that RequestId
3. Trace through service and database operations

---

## Performance Monitoring

Monitor these key metrics in logs:

| Metric | Threshold | Action if Exceeded |
|--------|-----------|-------------------|
| HTTP Response Duration | > 2000ms | Investigate slow operations |
| Database Query Duration | > 1000ms | Review database indexes |
| Service API Call Duration | > 5000ms | Check external API health |
| SaveChangesAsync Duration | > 500ms | Review transaction size |

---

## Security Considerations

⚠️ **Note:** Sensitive data logging is enabled only in Development mode:
- `EnableSensitiveDataLogging()` in DbContext
- Check `app.Environment.IsDevelopment()` conditions

**In Production:**
- Change log level from `Debug` to `Warning` or `Information`
- Disable sensitive data logging
- Secure log file storage and rotation

---

## Next Steps for Monitoring

1. **Dashboard Setup:** Integrate logs with ELK Stack, Splunk, or similar
2. **Alerts:** Configure alerts for Error/Fatal level logs
3. **Performance Tracking:** Create queries for request duration analysis
4. **Audit Trail:** Log all data modifications with user context
5. **Integration Tests:** Add tests that verify critical logging points

