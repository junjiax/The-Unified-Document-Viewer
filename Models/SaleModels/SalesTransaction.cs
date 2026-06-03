using System;
using System.Collections.Generic;

namespace Chap10.Models.SaleModels;

public partial class SalesTransaction
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

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<FinancingContract> FinancingContracts { get; set; } = new List<FinancingContract>();
}
