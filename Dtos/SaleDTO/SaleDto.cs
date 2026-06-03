namespace Chap10.Dtos.SaleDTO;

using System;

public class SaleDto
{
    public CustomerDto? Customer { get; set; }
    public SalesTransactionDto? SalesTransaction { get; set; }
    public SalesDocumentDto? SalesDocument { get; set; }
    public FinancingContractDto? FinancingContract { get; set; }
    public WarrantyRegistrationDto? WarrantyRegistration { get; set; }
}
