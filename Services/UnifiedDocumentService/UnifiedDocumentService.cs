using Chap10.Dtos.SaleDTO;
using Chap10.Dtos.ServiceDTO;
using Chap10.Dtos.UnifiedDTO;
using Chap10.Services.Shared;
using NuGet.Protocol;
using Serilog.Context;

namespace Chap10.Services;

public class UnifiedDocumentService : IUnifiedDocumentService
{
    private readonly ISaleApiClient _saleApi;
    private readonly IServiceApiClient _serviceApi;
    private readonly ILogger<UnifiedDocumentService> _logger;

    public UnifiedDocumentService(
        ISaleApiClient saleApi,
        IServiceApiClient serviceApi,
        ILogger<UnifiedDocumentService> logger)
    {
        _saleApi = saleApi;
        _serviceApi = serviceApi;
        _logger = logger;
    }

    public async Task<UnifiedDocumentDto> GetDocumentsByVinAsync(int vin)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("Starting GetDocumentsByVinAsync for VIN: {Vin} | OperationId: {OperationId}", vin, operationId);

            SaleDto? saleData = null;
            ServiceDto? serviceData = null;
            var errors = new List<string>();

            // Fetch Sale data
            try
            {
                _logger.LogInformation("Fetching sale data from Sale API for VIN: {Vin}", vin);
                saleData = await _saleApi.GetSaleDataByVinAsync(vin);
                
                if (saleData != null)
                {
                    _logger.LogInformation("Successfully retrieved sale data for VIN: {Vin}", vin);
                }
                else
                {
                    _logger.LogWarning("No sale data found for VIN: {Vin} - returning null from Sale API", vin);
                }
            }
            catch (HttpRequestException hEx)
            {
                _logger.LogError(hEx, "HTTP error while fetching sale data for VIN: {Vin} | Message: {ErrorMessage}", vin, hEx.Message);
                errors.Add($"Sale API error: {hEx.Message}");
            }
            catch (TaskCanceledException tEx)
            {
                _logger.LogError(tEx, "Timeout while fetching sale data for VIN: {Vin}", vin);
                errors.Add("Sale API timeout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching sale data for VIN: {Vin} | Error: {ErrorType}: {ErrorMessage}", vin, ex.GetType().Name, ex.Message);
                errors.Add($"Sale API error: {ex.Message}");
            }

            // Fetch Service data
            try
            {
                _logger.LogInformation("Fetching service data from Service API for VIN: {Vin}", vin);
                serviceData = await _serviceApi.GetServiceDataByVinAsync(vin);
                
                if (serviceData != null)
                {
                    _logger.LogInformation("Successfully retrieved service data for VIN: {Vin}", vin);
                }
                else
                {
                    _logger.LogWarning("No service data found for VIN: {Vin} - returning null from Service API", vin);
                }
            }
            catch (HttpRequestException hEx)
            {
                _logger.LogError(hEx, "HTTP error while fetching service data for VIN: {Vin} | Message: {ErrorMessage}", vin, hEx.Message);
                errors.Add($"Service API error: {hEx.Message}");
            }
            catch (TaskCanceledException tEx)
            {
                _logger.LogError(tEx, "Timeout while fetching service data for VIN: {Vin}", vin);
                errors.Add("Service API timeout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching service data for VIN: {Vin} | Error: {ErrorType}: {ErrorMessage}", vin, ex.GetType().Name, ex.Message);
                errors.Add($"Service API error: {ex.Message}");
            }

            // Log summary
            if (errors.Count > 0)
            {
                _logger.LogWarning("GetDocumentsByVinAsync completed with {ErrorCount} error(s) for VIN: {Vin} | Errors: {Errors}", 
                    errors.Count, vin, string.Join("; ", errors));
            }
            else
            {
                _logger.LogInformation("GetDocumentsByVinAsync completed successfully for VIN: {Vin} | Has Sale Data: {HasSaleData}, Has Service Data: {HasServiceData}",
                    vin, saleData != null, serviceData != null);
            }

            var result = new UnifiedDocumentDto
            {
                VIN = vin,
                SaleAPI = saleData,
                ServiceAPI = serviceData
            };

            return result;
        }
    }
}