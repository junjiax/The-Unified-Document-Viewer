using Chap10.Dtos.ServiceDTO;
using Chap10.Models.ServiceModels;
using Chap10.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace Chap10.Services;

public class ServiceAPI : IServiceApiClient
{
    private readonly ServiceDbContext _serviceContext;
    private readonly ILogger<ServiceAPI> _logger;

    public ServiceAPI(ServiceDbContext serviceContext, ILogger<ServiceAPI> logger)
    {
        _serviceContext = serviceContext;
        _logger = logger;
    }

    public async Task<ServiceDto?> GetServiceDataByVinAsync(int vin)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("Starting GetServiceDataByVinAsync for VIN: {Vin} | OperationId: {OperationId}", vin, operationId);

            try
            {
                // Fetch service record with related data
                _logger.LogDebug("Querying ServiceRecords table for VIN: {Vin}", vin);
                var serviceRecord = await _serviceContext.ServiceRecords
                    .Include(x => x.ServiceDocuments)
                    .Include(x => x.DiagnosticReports)
                    .Include(x => x.Technician)
                    .FirstOrDefaultAsync(x => x.VehicleId == vin);

                if (serviceRecord == null)
                {
                    _logger.LogInformation("No service record found for VIN: {Vin}", vin);
                    return null;
                }

                _logger.LogInformation("Found service record for VIN: {Vin} | ServiceRecordId: {ServiceRecordId} | ServiceType: {ServiceType} | Status: {ServiceStatus}",
                    vin, serviceRecord.ServiceRecordId, serviceRecord.ServiceType, serviceRecord.ServiceStatus);

                // Fetch most recent service document
                _logger.LogDebug("Extracting most recent service document for VIN: {Vin}", vin);
                var serviceDocument = serviceRecord.ServiceDocuments
                    .OrderByDescending(x => x.GeneratedDate)
                    .FirstOrDefault();

                if (serviceDocument != null)
                {
                    _logger.LogDebug("Found service document for VIN: {Vin} | DocumentId: {DocumentId} | Type: {DocumentType}",
                        vin, serviceDocument.ServiceDocumentId, serviceDocument.DocumentType);
                }
                else
                {
                    _logger.LogDebug("No service documents found for VIN: {Vin}", vin);
                }

                // Fetch most recent diagnostic report
                _logger.LogDebug("Extracting most recent diagnostic report for VIN: {Vin}", vin);
                var diagnosticReport = serviceRecord.DiagnosticReports
                    .OrderByDescending(x => x.GeneratedDate)
                    .FirstOrDefault();

                if (diagnosticReport != null)
                {
                    _logger.LogDebug("Found diagnostic report for VIN: {Vin} | ReportId: {ReportId} | SeverityLevel: {SeverityLevel} | Code: {DiagnosticCode}",
                        vin, diagnosticReport.DiagnosticReportId, diagnosticReport.SeverityLevel, diagnosticReport.DiagnosticCode);
                }
                else
                {
                    _logger.LogDebug("No diagnostic reports found for VIN: {Vin}", vin);
                }

                if (serviceRecord.Technician != null)
                {
                    _logger.LogDebug("Technician assigned to VIN: {Vin} | TechnicianId: {TechnicianId} | Name: {TechnicianName}",
                        vin, serviceRecord.Technician.TechnicianId, serviceRecord.Technician.FullName);
                }

                // Build response
                var result = new ServiceDto
                {
                    ServiceReport = new ServiceRecordDto
                    {
                        ServiceRecordId = serviceRecord.ServiceRecordId,
                        VehicleId = serviceRecord.VehicleId,
                        ServiceCenterId = serviceRecord.ServiceCenterId,
                        TechnicianId = serviceRecord.TechnicianId,
                        ServiceType = serviceRecord.ServiceType,
                        ServiceStatus = serviceRecord.ServiceStatus,
                        MileageAtService = serviceRecord.MileageAtService,
                        ServiceStartTime = serviceRecord.ServiceStartTime,
                        ServiceEndTime = serviceRecord.ServiceEndTime,
                        LaborCost = serviceRecord.LaborCost,
                        PartsCost = serviceRecord.PartsCost,
                        TotalCost = serviceRecord.TotalCost,
                        CustomerComplaint = serviceRecord.CustomerComplaint,
                        TechnicianNotes = serviceRecord.TechnicianNotes,
                        CreatedAt = serviceRecord.CreatedAt,
                        UpdatedAt = serviceRecord.UpdatedAt
                    },
                    Technician = serviceRecord.Technician == null ? null : new TechnicianDto
                    {
                        TechnicianId = serviceRecord.Technician.TechnicianId,
                        FullName = serviceRecord.Technician.FullName,
                        EmployeeCode = serviceRecord.Technician.EmployeeCode,
                        CertificationLevel = serviceRecord.Technician.CertificationLevel,
                        Specialization = serviceRecord.Technician.Specialization,
                        YearsOfExperience = serviceRecord.Technician.YearsOfExperience,
                        ContactNumber = serviceRecord.Technician.ContactNumber,
                        EmploymentStatus = serviceRecord.Technician.EmploymentStatus,
                        CreatedAt = serviceRecord.Technician.CreatedAt,
                        UpdatedAt = serviceRecord.Technician.UpdatedAt
                    },
                    ServiceDocument = serviceDocument == null ? null : new ServiceDocumentDto
                    {
                        ServiceDocumentId = serviceDocument.ServiceDocumentId,
                        VehicleId = serviceDocument.VehicleId,
                        DocumentType = serviceDocument.DocumentType,
                        DocumentTitle = serviceDocument.DocumentTitle,
                        FileUrl = serviceDocument.FileUrl,
                        FileFormat = serviceDocument.FileFormat,
                        SourceSystem = serviceDocument.SourceSystem,
                        UploadedBy = serviceDocument.UploadedBy,
                        GeneratedDate = serviceDocument.GeneratedDate,
                        Status = serviceDocument.Status,
                        CreatedAt = serviceDocument.CreatedAt,
                        UpdatedAt = serviceDocument.UpdatedAt
                    },
                    DiagnosticReport = diagnosticReport == null ? null : new DiagnosticReportDto
                    {
                        DiagnosticReportId = diagnosticReport.DiagnosticReportId,
                        ServiceRecordId = diagnosticReport.ServiceRecordId,
                        VehicleId = diagnosticReport.VehicleId,
                        DiagnosticCode = diagnosticReport.DiagnosticCode,
                        SeverityLevel = diagnosticReport.SeverityLevel,
                        ProblemDescription = diagnosticReport.ProblemDescription,
                        RecommendedAction = diagnosticReport.RecommendedAction,
                        GeneratedBy = diagnosticReport.GeneratedBy,
                        GeneratedDate = diagnosticReport.GeneratedDate,
                        ResolutionStatus = diagnosticReport.ResolutionStatus,
                        CreatedAt = diagnosticReport.CreatedAt,
                        UpdatedAt = diagnosticReport.UpdatedAt
                    }
                };

                _logger.LogInformation("Successfully built service data for VIN: {Vin} | Has Technician: {HasTechnician}, Has DiagnosticReport: {HasDiagnosticReport}",
                    vin, result.Technician != null, result.DiagnosticReport != null);

                return result;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while fetching service data for VIN: {Vin} | Message: {ErrorMessage}", vin, dbEx.Message);
                throw;
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "Invalid operation while fetching service data for VIN: {Vin} | Message: {ErrorMessage}", vin, invOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching service data for VIN: {Vin} | Error: {ErrorType}: {ErrorMessage}",
                    vin, ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}
