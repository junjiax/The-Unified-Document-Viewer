using System;

namespace Chap10.Dtos.ServiceDTO;

public class DiagnosticReportDto
{
    public int DiagnosticReportId { get; set; }
    public int? ServiceRecordId { get; set; }
    public int? VehicleId { get; set; }
    public string? DiagnosticCode { get; set; }
    public string? SeverityLevel { get; set; }
    public string? ProblemDescription { get; set; }
    public string? RecommendedAction { get; set; }
    public int? GeneratedBy { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public string? ResolutionStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
