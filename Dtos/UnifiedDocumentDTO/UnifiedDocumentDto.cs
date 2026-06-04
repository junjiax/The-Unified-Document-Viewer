namespace Chap10.Dtos.UnifiedDTO;
using Chap10.Dtos.SaleDTO;
using Chap10.Dtos.ServiceDTO;

public class UnifiedDocumentDto
{
    public int? VIN { get; set; }

    public SaleDto? SaleAPI { get; set; }

    public ServiceDto? ServiceAPI { get; set; }

}