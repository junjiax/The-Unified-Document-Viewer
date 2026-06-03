using System;

namespace Chap10.Dtos.SaleDTO;

public class FinancingContractDto
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
}
