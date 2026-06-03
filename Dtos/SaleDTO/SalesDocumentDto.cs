using System;

namespace Chap10.Dtos.SaleDTO;

public class SalesDocumentDto
{
    public int SalesDocumentId { get; set; }
    public int? VehicleId { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? DocumentTitle { get; set; }
    public string? FileUrl { get; set; }
    public string? FileFormat { get; set; }
    public string? SourceSystem { get; set; }
    public Guid? UploadedBy { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public string? Status { get; set; }
    public int? Version { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
