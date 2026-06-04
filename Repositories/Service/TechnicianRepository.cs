using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class TechnicianRepository : GenericRepository<Technician>
{
    public TechnicianRepository(ServiceDbContext db, ILogger<GenericRepository<Technician>> logger) : base(db, logger)
    {
    }
}
