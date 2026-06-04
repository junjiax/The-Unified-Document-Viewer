using Chap10.Dtos.SaleDTO;

namespace Chap10.Services.Shared;

public interface ISaleApiClient
{
    Task<SaleDto?> GetSaleDataByVinAsync(int vin);
}