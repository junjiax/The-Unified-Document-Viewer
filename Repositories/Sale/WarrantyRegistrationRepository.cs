using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class WarrantyRegistrationRepository : GenericRepository<WarrantyRegistration>
{
    public WarrantyRegistrationRepository(SaleDbContext db, ILogger<GenericRepository<WarrantyRegistration>> logger) : base(db, logger)
    {
    }
}
