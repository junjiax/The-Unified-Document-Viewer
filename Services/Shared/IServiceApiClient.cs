using Chap10.Dtos.ServiceDTO;

namespace Chap10.Services.Shared;

public interface IServiceApiClient
{
    Task<ServiceDto?> GetServiceDataByVinAsync(int vin);
}