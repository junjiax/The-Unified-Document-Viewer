using System;
using System.Collections.Generic;

namespace Chap10.Models.ServiceModels;

public partial class DiagnosticReport
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

    public virtual ServiceRecord? ServiceRecord { get; set; }
}
