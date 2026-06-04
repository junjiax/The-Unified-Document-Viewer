using Chap10.Dtos.UnifiedDTO;
using Microsoft.AspNetCore.Mvc;
using Chap10.Services.Shared;
using Chap10.Dtos;
using Serilog.Context;
using NuGet.Protocol;

namespace Chap10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnifiedDocumentController : ControllerBase
{
    private readonly IUnifiedDocumentService _service;
    private readonly ILogger<UnifiedDocumentController> _logger;

    public UnifiedDocumentController(IUnifiedDocumentService service, ILogger<UnifiedDocumentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public class UnifiedDocumentRequest
    {
        public string? Vin { get; set; }
    }

    [HttpGet("{vin}")]
    public async Task<ActionResult<APIResponse<UnifiedDocumentDto>>> GetByVin(
        string vin)
    {
        var requestId = HttpContext.TraceIdentifier;
        using (LogContext.PushProperty("RequestId", requestId))
        {
            _logger.LogInformation("GetByVin endpoint called with VIN: {Vin} | RequestId: {RequestId}", vin, requestId);

            if (string.IsNullOrWhiteSpace(vin))
            {
                _logger.LogWarning("GetByVin called with null or empty VIN | RequestId: {RequestId}", requestId);
                return BadRequest(APIResponse<UnifiedDocumentDto>.Fail("VIN is required", null));
            }

            try
            {
                _logger.LogDebug("Attempting to parse VIN: {Vin} to integer | RequestId: {RequestId}", vin, requestId);

                if (!int.TryParse(vin, out int vinNumber))
                {
                    _logger.LogWarning("Failed to parse VIN to integer. VIN: {Vin} | RequestId: {RequestId}", vin, requestId);
                    return BadRequest(APIResponse<UnifiedDocumentDto>.Fail("VIN must be a valid integer", null));
                }

                _logger.LogInformation("Starting document retrieval for VIN: {VinNumber} | RequestId: {RequestId}", vinNumber, requestId);

                var documents = await _service.GetDocumentsByVinAsync(vinNumber);

                if (documents == null)
                {
                    _logger.LogWarning("No documents found for VIN: {VinNumber} | RequestId: {RequestId}", vinNumber, requestId);
                    return NotFound(APIResponse<UnifiedDocumentDto>.Fail($"No documents found for VIN: {vinNumber}", null));
                }

                _logger.LogInformation("Successfully retrieved documents for VIN: {VinNumber} | RequestId: {RequestId}", vinNumber, requestId);

                var result = new UnifiedDocumentDto
                {
                    VIN = vinNumber,
                    SaleAPI = documents.SaleAPI,
                    ServiceAPI = documents.ServiceAPI
                };
                
                _logger.LogDebug("UnifiedDocumentDto created for VIN: {VinNumber} | RequestId: {RequestId} | Result: {@Result}", vinNumber, requestId, result);
                return Ok(APIResponse<UnifiedDocumentDto>.Ok(result, "Documents retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving documents for VIN: {Vin} | RequestId: {RequestId}", vin, requestId);
                return StatusCode(500, APIResponse<UnifiedDocumentDto>.Fail("An error occurred while processing your request.", null));
            }
        }
    }
}
