using Chap10.Models.SaleModels;
using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class FinancingContractRepository : GenericRepository<FinancingContract>
{
    public FinancingContractRepository(SaleDbContext db) : base(db)
    {
    }
}
