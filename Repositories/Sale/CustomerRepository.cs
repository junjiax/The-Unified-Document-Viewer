using Chap10.Models.SaleModels;
using Microsoft.EntityFrameworkCore;

namespace Chap10.Repositories.Sale;

public class CustomerRepository : GenericRepository<Customer>
{
    private readonly SaleDbContext _db;
    public CustomerRepository(SaleDbContext db, ILogger<GenericRepository<Customer>> logger) : base(db, logger)
    {
        _db = db;
    }
}
