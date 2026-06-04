# Logging Flow Diagram & Architecture

## Complete Request Flow with Logging Points

```
HTTP Request: GET /api/UnifiedDocument/12345
│
├─────────────────────────────────────────────────────────────────────────
│ MIDDLEWARE LAYER - RequestLoggingMiddleware
│ ✅ LOG: "HTTP GET request initiated: /api/UnifiedDocument/12345"
│ ✅ LOG: ClientIP, RequestId, User context
│ ✅ START: Timer for request duration
│
├─────────────────────────────────────────────────────────────────────────
│ MIDDLEWARE LAYER - Exception Handler Setup
│ ✅ Wrapped for exception capture
│
├─────────────────────────────────────────────────────────────────────────
│ CONTROLLER LAYER
│ │
│ ├─ UnifiedDocumentController.GetByVin("12345")
│ │  ✅ LOG: "GetByVin endpoint called with VIN: 12345"
│ │  ✅ LOG: RequestId: {requestId}
│ │  
│ │  ├─ Validate Input
│ │  │  ✅ LOG: "Parsing VIN: 12345 to integer"
│ │  │  ✅ LOG: VIN number: 12345
│ │  │
│ │  ├─ Call Service
│ │  │  ✅ LOG: "Starting document retrieval for VIN: 12345"
│ │  │
│ │  └─ Handle Response/Error
│ │     ✅ LOG: "Successfully retrieved documents for VIN: 12345"
│ │     OR
│ │     ✅ LOG ERROR: Specific exception type and message
│
├─────────────────────────────────────────────────────────────────────────
│ SERVICE LAYER - UnifiedDocumentService
│ │
│ ├─ GetDocumentsByVinAsync(12345)
│ │  ✅ LOG: "Starting GetDocumentsByVinAsync for VIN: 12345"
│ │  ✅ LOG: OperationId: {operationId}
│ │  
│ │  ├─ SaleAPI Call
│ │  │  │
│ │  │  ├─ SaleAPI.GetSaleDataByVinAsync(12345)
│ │  │  │  ✅ LOG: "Starting GetSaleDataByVinAsync for VIN: 12345"
│ │  │  │  
│ │  │  │  ├─ Database Queries
│ │  │  │  │  ├─ SalesTransactions table
│ │  │  │  │  │  ✅ LOG: "Querying SalesTransactions table for VIN: 12345"
│ │  │  │  │  │  ✅ LOG: "Found sales transaction"
│ │  │  │  │  │  OR
│ │  │  │  │  │  ✅ LOG: "No sales transaction found"
│ │  │  │  │  │
│ │  │  │  │  ├─ SalesDocuments table
│ │  │  │  │  │  ✅ LOG: "Querying SalesDocuments table"
│ │  │  │  │  │  ✅ LOG: "Found sales document" (if exists)
│ │  │  │  │  │
│ │  │  │  │  └─ WarrantyRegistrations table
│ │  │  │  │     ✅ LOG: "Querying WarrantyRegistrations table"
│ │  │  │  │     ✅ LOG: "Found warranty registration" (if exists)
│ │  │  │  │
│ │  │  │  ├─ Data Mapping
│ │  │  │  │  ✅ LOG: "Successfully built sale data for VIN: 12345"
│ │  │  │  │
│ │  │  │  └─ Return
│ │  │  │     ✅ LOG: "Successfully retrieved sale data"
│ │  │  │     OR (if timeout/HTTP error)
│ │  │  │     ✅ LOG ERROR: HttpRequestException / TaskCanceledException
│ │  │  │
│ │  │  └─ Back to Service
│ │  │     ✅ LOG: "Successfully retrieved sale data for VIN: 12345"
│ │  │     OR
│ │  │     ✅ LOG ERROR: Specific error and message
│ │  │
│ │  ├─ ServiceAPI Call (parallel)
│ │  │  │
│ │  │  ├─ ServiceAPI.GetServiceDataByVinAsync(12345)
│ │  │  │  ✅ LOG: "Starting GetServiceDataByVinAsync for VIN: 12345"
│ │  │  │  
│ │  │  │  ├─ Database Queries
│ │  │  │  │  ├─ ServiceRecords table
│ │  │  │  │  │  ✅ LOG: "Querying ServiceRecords table"
│ │  │  │  │  │  ✅ LOG: "Found service record"
│ │  │  │  │  │  OR
│ │  │  │  │  │  ✅ LOG: "No service record found"
│ │  │  │  │  │
│ │  │  │  │  ├─ ServiceDocuments table
│ │  │  │  │  │  ✅ LOG: "Extracting service document"
│ │  │  │  │  │  ✅ LOG: "Found service document" (if exists)
│ │  │  │  │  │
│ │  │  │  │  ├─ DiagnosticReports table
│ │  │  │  │  │  ✅ LOG: "Extracting diagnostic report"
│ │  │  │  │  │  ✅ LOG: "Found diagnostic report" (if exists)
│ │  │  │  │  │
│ │  │  │  │  └─ Technician table
│ │  │  │  │     ✅ LOG: "Technician assigned" (if exists)
│ │  │  │  │
│ │  │  │  ├─ Data Mapping
│ │  │  │  │  ✅ LOG: "Successfully built service data"
│ │  │  │  │
│ │  │  │  └─ Return
│ │  │  │     ✅ LOG: "Successfully retrieved service data"
│ │  │  │     OR (if timeout/HTTP error)
│ │  │  │     ✅ LOG ERROR: DbUpdateException / InvalidOperationException
│ │  │  │
│ │  │  └─ Back to Service
│ │  │     ✅ LOG: "Successfully retrieved service data for VIN: 12345"
│ │  │     OR
│ │  │     ✅ LOG ERROR: Specific error and message
│ │  │
│ │  ├─ Combine Results
│ │  │  ✅ LOG: "GetDocumentsByVinAsync completed successfully"
│ │  │  ✅ LOG: "Has Sale Data: True, Has Service Data: True"
│ │  │  (or combinations of True/False based on what was returned)
│ │  │
│ │  └─ Return UnifiedDocumentDto
│ │
│ └─ Back to Controller
│
├─────────────────────────────────────────────────────────────────────────
│ REPOSITORY LAYER (if called directly)
│ 
│ Example: CustomerRepository.GetByIdAsync(100)
│ │
│ ├─ GenericRepository<Customer>.GetByIdAsync(100)
│ │  ✅ LOG: "GetByIdAsync called for entity type: Customer with id: 100"
│ │  ✅ LOG: OperationId: {operationId}
│ │  
│ │  ├─ Find Entity
│ │  │  ✅ LOG: "Entity found: Customer with id: 100"
│ │  │  OR
│ │  │  ✅ LOG WARNING: "Entity not found: Customer with id: 100"
│ │  │
│ │  └─ Return Entity
│ │
│ └─ Back to Caller
│
├─────────────────────────────────────────────────────────────────────────
│ RESPONSE BUILD & SEND
│ ✅ LOG: "Successfully retrieved documents for VIN: 12345"
│ ✅ HTTP 200 OK
│ ✅ Response body: {"VIN": 12345, "SaleAPI": {...}, "ServiceAPI": {...}}
│
├─────────────────────────────────────────────────────────────────────────
│ MIDDLEWARE LAYER - RequestLoggingMiddleware
│ ✅ LOG: "HTTP response completed: GET /api/UnifiedDocument/12345"
│ ✅ LOG: "StatusCode: 200"
│ ✅ LOG: "Duration: 750ms"
│ ✅ LOG: RequestId: {same as request}
│
└─────────────────────────────────────────────────────────────────────────
HTTP Response Sent
```

---

## Error Flow Example (404 - Not Found)

```
HTTP Request: GET /api/UnifiedDocument/99999
│
├─ RequestLoggingMiddleware START
│  ✅ LOG: "HTTP GET request initiated: /api/UnifiedDocument/99999"
│
├─ UnifiedDocumentController.GetByVin("99999")
│  ✅ LOG: "GetByVin endpoint called with VIN: 99999"
│  │
│  ├─ UnifiedDocumentService.GetDocumentsByVinAsync(99999)
│  │  ✅ LOG: "Starting GetDocumentsByVinAsync for VIN: 99999"
│  │  │
│  │  ├─ SaleAPI.GetSaleDataByVinAsync(99999)
│  │  │  ✅ LOG: "Found sales transaction for VIN: 99999: FALSE"
│  │  │  ✅ LOG: "No sales transaction found for VIN: 99999"
│  │  │  → Returns null
│  │  │
│  │  ├─ ServiceAPI.GetServiceDataByVinAsync(99999)
│  │  │  ✅ LOG: "Found service record for VIN: 99999: FALSE"
│  │  │  ✅ LOG: "No service record found for VIN: 99999"
│  │  │  → Returns null
│  │  │
│  │  └─ Service returns UnifiedDocumentDto with both nulls
│  │     ✅ LOG: "GetDocumentsByVinAsync completed with 0 error(s)"
│  │     ✅ LOG: "Has Sale Data: False, Has Service Data: False"
│  │
│  ├─ Check result null
│  │  ✅ LOG WARNING: "No documents found for VIN: 99999"
│  │  → Return 404 NotFound
│  │
│  └─ HTTP 404 Not Found
│
├─ RequestLoggingMiddleware END
│  ✅ LOG: "HTTP response completed: GET /api/UnifiedDocument/99999"
│  ✅ LOG: "StatusCode: 404"
│  ✅ LOG: "Duration: 150ms"
│
└─ HTTP Response Sent: 404 Not Found
```

---

## Exception Flow Example (502 - Service Unavailable)

```
HTTP Request: GET /api/UnifiedDocument/12345
│
├─ UnifiedDocumentController.GetByVin("12345")
│
├─ UnifiedDocumentService.GetDocumentsByVinAsync(12345)
│  │
│  ├─ SaleAPI.GetSaleDataByVinAsync(12345)
│  │  │
│  │  └─ ❌ HttpRequestException: "Unable to connect to external API"
│  │     ✅ LOG ERROR: "HTTP error occurred while retrieving documents"
│  │     ✅ LOG ERROR: Exception stack trace
│  │     → Returns null
│  │
│  └─ Service catches and logs: "Sale API error: Unable to connect..."
│
├─ ExceptionHandlingMiddleware catches (if unhandled)
│  ✅ LOG ERROR: "Unhandled exception occurred during request processing"
│  → Categorizes as HttpRequestException
│  → HTTP 502 Bad Gateway
│
└─ HTTP 502 Response
```

---

## Log File Structure

```
logs/
│
├── unified-document-viewer-2024-01-15.txt (Current)
│   ├── [2024-01-15 10:00:00.000 +05:00] [INF] Starting Unified Document Viewer application
│   ├── [2024-01-15 10:00:05.123 +05:00] [INF] HTTP GET request initiated: /api/UnifiedDocument/12345
│   ├── [2024-01-15 10:00:05.125 +05:00] [INF] GetByVin endpoint called with VIN: 12345
│   ├── [2024-01-15 10:00:05.130 +05:00] [DBG] Attempting to parse VIN: 12345 to integer
│   ├── [2024-01-15 10:00:05.135 +05:00] [INF] Starting GetDocumentsByVinAsync for VIN: 12345
│   ├── [2024-01-15 10:00:05.140 +05:00] [INF] Fetching sale data from Sale API for VIN: 12345
│   ├── [2024-01-15 10:00:05.145 +05:00] [INF] Starting GetSaleDataByVinAsync for VIN: 12345
│   ├── [2024-01-15 10:00:05.150 +05:00] [DBG] Querying SalesTransactions table for VIN: 12345
│   ├── [2024-01-15 10:00:05.250 +05:00] [INF] Found sales transaction for VIN: 12345
│   ├── ... (more logs for other queries)
│   ├── [2024-01-15 10:00:05.850 +05:00] [INF] Successfully retrieved documents for VIN: 12345
│   ├── [2024-01-15 10:00:05.855 +05:00] [INF] HTTP response completed: GET /api/UnifiedDocument/12345 | StatusCode: 200 | Duration: 750ms
│   │
│   └── ... (more requests)
│
├── unified-document-viewer-2024-01-14.txt (Previous day)
└── ... (older files, up to 7 days retention)
```

---

## Correlation ID Tracking

```
Request Flow with IDs:
│
├─ HTTP Request arrives
│  └─ RequestId: 0HN5K4F1D5GJR:00000001 (Generated by ASP.NET Core)
│
├─ UnifiedDocumentService
│  └─ OperationId: a1b2c3d4 (Generated in service)
│     Logs: "... | OperationId: a1b2c3d4"
│
│  ├─ SaleAPI
│  │  └─ OperationId: x1y2z3a4 (Generated in SaleAPI)
│  │     Logs: "... | OperationId: x1y2z3a4"
│  │
│  │  └─ GenericRepository<Customer>
│  │     └─ OperationId: cust001 (Generated in repository)
│  │        Logs: "... | OperationId: cust001"
│  │
│  └─ ServiceAPI
│     └─ OperationId: p1q2r3s4 (Generated in ServiceAPI)
│        Logs: "... | OperationId: p1q2r3s4"
│
└─ Middleware captures RequestId
   ✅ Correlates all logs for this request

Search Strategy:
  1. Find RequestId in response headers or logs
  2. Search logs: "RequestId: 0HN5K4F1D5GJR:00000001"
  3. Follow OperationIds through layers
  4. Reconstruct complete flow from logs
```

---

## Performance Timeline Example

```
Timeline for VIN 12345 Request:

T+0ms:     Request arrives
           [INF] HTTP GET request initiated

T+5ms:     Controller processing
           [INF] GetByVin endpoint called
           [DBG] Attempting to parse VIN

T+10ms:    Service layer
           [INF] Starting GetDocumentsByVinAsync

T+15ms:    SaleAPI starts
           [INF] Fetching sale data from Sale API

T+20ms:    Database query
           [DBG] Querying SalesTransactions table

T+150ms:   SaleAPI completes
           [INF] Successfully retrieved sale data

T+160ms:   ServiceAPI starts
           [INF] Fetching service data

T+200ms:   Database queries
           [DBG] Querying ServiceRecords table

T+300ms:   ServiceAPI completes
           [INF] Successfully retrieved service data

T+350ms:   Service combines results
           [INF] GetDocumentsByVinAsync completed successfully

T+355ms:   Response building
           [INF] Successfully retrieved documents

T+360ms:   Response sent
           [INF] HTTP response completed
           [INF] Duration: 360ms

Bottleneck: SaleAPI (150ms) and ServiceAPI (150ms)
Optimization: Run in parallel or cache results
```

---

## Exception Categorization Tree

```
All Exceptions
│
├─ ArgumentNullException
│  └─ HTTP 400 Bad Request
│     Log: "Invalid argument: {ParamName}"
│
├─ ArgumentException
│  └─ HTTP 400 Bad Request
│     Log: "Invalid input: {Message}"
│
├─ InvalidOperationException
│  └─ HTTP 409 Conflict
│     Log: "Invalid operation"
│
├─ TimeoutException
│  └─ HTTP 504 Gateway Timeout
│     Log: "Request timeout"
│
├─ HttpRequestException
│  └─ HTTP 502 Bad Gateway
│     Log: "External service error"
│
├─ DbUpdateException
│  └─ Logged but rethrown
│     Log: "Database error"
│
├─ DbUpdateConcurrencyException
│  └─ Logged but rethrown
│     Log: "Concurrency error"
│
└─ All Others
   └─ HTTP 500 Internal Server Error
      Log: "Unexpected error"
```

---

## Search Patterns for Log Analysis

```
# Find all requests for a VIN
grep "VIN: 12345" logs/*.txt

# Find slow requests (> 2 seconds)
grep "Duration: [2-9][0-9][0-9][0-9]" logs/*.txt

# Find all errors
grep "\[ERR\]\|\[FATAL\]" logs/*.txt

# Find timeout issues
grep "TimeoutException\|TaskCanceledException" logs/*.txt

# Find database errors
grep "DbUpdate\|DbConcurrency" logs/*.txt

# Find a specific request by ID
grep "0HN5K4F1D5GJR:00000001" logs/*.txt

# Find all operations in trace
grep "OperationId: a1b2c3d4" logs/*.txt

# Find failed operations
grep "ERROR\|WARNING" logs/*.txt

# Count log levels
grep -o "\[INF\]\|\[WRN\]\|\[ERR\]" logs/*.txt | sort | uniq -c

# Find external API errors
grep "SaleAPI\|ServiceAPI" logs/*.txt | grep "ERROR\|error"
```

