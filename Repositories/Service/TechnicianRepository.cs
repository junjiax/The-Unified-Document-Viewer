using Chap10.Models.ServiceModels;
using Chap10.Models.ServiceModels;

namespace Chap10.Repositories.Service;

public class TechnicianRepository : GenericRepository<Technician>
{
    public TechnicianRepository(ServiceDbContext db) : base(db)
    {
    }
}
