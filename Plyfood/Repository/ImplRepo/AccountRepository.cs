using Plyfood.Context;
using Plyfood.Entity;

namespace Plyfood.Repository.ImplRepo;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Account> GetAll()
    {
        throw new NotImplementedException();
    }

    public Account GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Add(Account entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Account entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
}