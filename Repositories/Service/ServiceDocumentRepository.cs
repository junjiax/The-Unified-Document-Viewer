using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class ServiceDocumentRepository : GenericRepository<ServiceDocument>
{
    public ServiceDocumentRepository(ServiceDbContext db, ILogger<GenericRepository<ServiceDocument>> logger) : base(db, logger)
    {
    }
}
