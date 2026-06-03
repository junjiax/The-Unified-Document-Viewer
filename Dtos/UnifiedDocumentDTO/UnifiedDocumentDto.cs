namespace Chap10.Dtos.UnifiedDTO;
using Chap10.Dtos.SaleDTO;
using Chap10.Dtos.ServiceDTO;

public class UnifiedDocumentDto
{
    public int? Vin { get; set; }

    public SaleDto? Sale { get; set; }

    public ServiceDto? Service { get; set; }

}