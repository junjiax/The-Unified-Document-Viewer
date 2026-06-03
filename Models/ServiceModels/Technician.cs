using System;
using System.Collections.Generic;

namespace Chap10.Models.ServiceModels;

public partial class Technician
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

    public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
}
