using Chap10.Models.ServiceModels;
using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class ServiceRecordRepository : GenericRepository<ServiceRecord>
{
    public ServiceRecordRepository(ServiceDbContext db) : base(db)
    {
    }
}
