using System;

namespace Chap10.Dtos.ServiceDTO;

public class ServiceRecordDto
{
    public int ServiceRecordId { get; set; }
    public int? VehicleId { get; set; }
    public int? ServiceCenterId { get; set; }
    public int? TechnicianId { get; set; }
    public string? ServiceType { get; set; }
    public string? ServiceStatus { get; set; }
    public int? MileageAtService { get; set; }
    public DateTime? ServiceStartTime { get; set; }
    public DateTime? ServiceEndTime { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public decimal? TotalCost { get; set; }
    public string? CustomerComplaint { get; set; }
    public string? TechnicianNotes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
