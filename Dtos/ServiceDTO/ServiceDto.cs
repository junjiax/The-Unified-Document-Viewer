namespace Chap10.Dtos.ServiceDTO;

public class ServiceDto
{
    public DiagnosticReportDto? DiagnosticReport { get; set; }
    public ServiceDocumentDto? ServiceDocument { get; set; }
    public ServiceRecordDto? ServiceReport { get; set; }
    public TechnicianDto? Technician { get; set; }

}
