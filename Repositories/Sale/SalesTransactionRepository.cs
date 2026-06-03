using Chap10.Models.SaleModels;
using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class SalesTransactionRepository : GenericRepository<SalesTransaction>
{
    public SalesTransactionRepository(SaleDbContext db) : base(db)
    {
    }
}
