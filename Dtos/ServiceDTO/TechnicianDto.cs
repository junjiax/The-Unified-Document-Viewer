using System;

namespace Chap10.Dtos.ServiceDTO;

public class TechnicianDto
{
    public int TechnicianId { get; set; }
    public string? FullName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? CertificationLevel { get; set; }
    public string? Specialization { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? ContactNumber { get; set; }
    public string? EmploymentStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
