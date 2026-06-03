using Chap10.Dtos.UnifiedDTO;

namespace Chap10.Services.Shared;

public interface IUnifiedDocumentService
{
    Task<UnifiedDocumentDto> GetDocumentsByVinAsync(int vin);
}