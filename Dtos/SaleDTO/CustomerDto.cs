using System;

namespace Chap10.Dtos.SaleDTO;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? CustomerType { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? LoyaltyTier { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
