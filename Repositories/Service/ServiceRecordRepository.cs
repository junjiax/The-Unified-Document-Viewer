using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class ServiceRecordRepository : GenericRepository<ServiceRecord>
{
    public ServiceRecordRepository(ServiceDbContext db, ILogger<GenericRepository<ServiceRecord>> logger) : base(db, logger)
    {
    }
}
