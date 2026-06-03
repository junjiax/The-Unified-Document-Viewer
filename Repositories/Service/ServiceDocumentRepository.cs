using Chap10.Models.ServiceModels;
using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class ServiceDocumentRepository : GenericRepository<ServiceDocument>
{
    public ServiceDocumentRepository(ServiceDbContext db) : base(db)
    {
    }
}
