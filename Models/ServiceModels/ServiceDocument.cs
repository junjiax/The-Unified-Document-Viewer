using System;
using System.Collections.Generic;

namespace Chap10.Models.ServiceModels;

public partial class ServiceDocument
{
    public int ServiceDocumentId { get; set; }

    public int? VehicleId { get; set; }

    public int? ServiceRecordId { get; set; }

    public string? DocumentType { get; set; }

    public string? DocumentTitle { get; set; }

    public string? FileUrl { get; set; }

    public string? FileFormat { get; set; }

    public string? SourceSystem { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public int? UploadedBy { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ServiceRecord? ServiceRecord { get; set; }
}
