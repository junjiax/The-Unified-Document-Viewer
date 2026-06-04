using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class SalesTransactionRepository : GenericRepository<SalesTransaction>
{
    public SalesTransactionRepository(SaleDbContext db, ILogger<GenericRepository<SalesTransaction>> logger) : base(db, logger)
    {
    }
}
