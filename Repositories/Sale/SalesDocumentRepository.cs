using Chap10.Models.SaleModels;
using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class SalesDocumentRepository : GenericRepository<SalesDocument>
{
    public SalesDocumentRepository(SaleDbContext db) : base(db)
    {
    }
}
