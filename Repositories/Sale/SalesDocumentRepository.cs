using Chap10.Models.SaleModels;

namespace Chap10.Repositories.Sale;

public class SalesDocumentRepository : GenericRepository<SalesDocument>
{
    public SalesDocumentRepository(SaleDbContext db, ILogger<GenericRepository<SalesDocument>> logger) : base(db, logger)
    {
    }
}
