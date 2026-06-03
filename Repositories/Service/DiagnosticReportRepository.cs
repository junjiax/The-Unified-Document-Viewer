using Chap10.Models.ServiceModels;
using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class DiagnosticReportRepository : GenericRepository<DiagnosticReport>
{
    public DiagnosticReportRepository(ServiceDbContext db) : base(db)
    {
    }
}
