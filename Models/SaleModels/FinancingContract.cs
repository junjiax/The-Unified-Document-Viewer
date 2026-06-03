using System;
using System.Collections.Generic;

namespace Chap10.Models.SaleModels;

public partial class FinancingContract
{
    public int FinancingContractId { get; set; }

    public int? TransactionId { get; set; }

    public string? FinancingProvider { get; set; }

    public decimal? LoanAmount { get; set; }

    public decimal? InterestRate { get; set; }

    public int? LoanDurationMonths { get; set; }

    public decimal? MonthlyPayment { get; set; }

    public DateTime? ContractStartDate { get; set; }

    public DateTime? ContractEndDate { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SalesTransaction? Transaction { get; set; }
}
