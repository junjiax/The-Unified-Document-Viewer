using System;

namespace Chap10.Dtos.SaleDTO;

public class SalesTransactionDto
{
    public int TransactionId { get; set; }
    public int? VehicleId { get; set; }
    public int? CustomerId { get; set; }
    public int? SalesRepresentativeId { get; set; }
    public int? DealershipId { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionStatus { get; set; }
    public DateTime? SalesDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
