using System;

namespace Chap10.Dtos.ServiceDTO;

public class ServiceDocumentDto
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
}
