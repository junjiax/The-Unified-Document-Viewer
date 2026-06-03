using Chap10.Dtos.SaleDTO;
using Chap10.Dtos.ServiceDTO;
using Chap10.Dtos.UnifiedDTO;
using Chap10.Models.SaleModels;
using Chap10.Models.ServiceModels;
using Chap10.Services.Shared;
using Microsoft.EntityFrameworkCore;

namespace Chap10.Services;
public class UnifiedDocumentService : IUnifiedDocumentService
{
    private readonly SaleDbContext _saleContext;
    private readonly ServiceDbContext _serviceContext;

    public UnifiedDocumentService(
        SaleDbContext saleContext,
        ServiceDbContext serviceContext)
    {
        _saleContext = saleContext;
        _serviceContext = serviceContext;
    }

    public async Task<UnifiedDocumentDto> GetDocumentsByVinAsync(int vin)
    {
        var sales = await _saleContext.SalesDocuments
            .Where(x => x.VehicleId == vin)
            .ToListAsync();

        var serviceDocuments = await _serviceContext.ServiceDocuments
            .Where(x => x.VehicleId == vin)
            .ToListAsync();

        UnifiedDocumentDto unifiedDocument = new UnifiedDocumentDto
        {
            Vin = vin,
            Sale = sales.Select(x => new SaleDto
            {
                SalesDocument = new SalesDocumentDto
                {
                    SalesDocumentId = x.SalesDocumentId,
                    VehicleId = x.VehicleId,
                    DocumentType = x.DocumentType,
                    DocumentNumber = x.DocumentNumber,
                    DocumentTitle = x.DocumentTitle,
                    FileUrl = x.FileUrl,
                    FileFormat = x.FileFormat,
                    SourceSystem = x.SourceSystem,
                    UploadedBy = x.UploadedBy,
                    GeneratedDate = x.GeneratedDate,
                    Status = x.Status,
                    Version = x.Version,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }
            }).FirstOrDefault(),
            Service = serviceDocuments.Select(x => new ServiceDto
            {
                ServiceDocument = new ServiceDocumentDto
                {
                    ServiceDocumentId = x.ServiceDocumentId,
                    VehicleId = x.VehicleId,
                    DocumentType = x.DocumentType,
                    DocumentTitle = x.DocumentTitle,
                    FileUrl = x.FileUrl,
                    FileFormat = x.FileFormat,
                    SourceSystem = x.SourceSystem,
                    UploadedBy = x.UploadedBy,
                    GeneratedDate = x.GeneratedDate,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }
            }).FirstOrDefault()
        };
        return unifiedDocument;
    }
}