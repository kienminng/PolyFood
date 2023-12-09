
using Plyfood.Context;
using Plyfood.Entity;
using Plyfood.Reposioty.IRepo;

namespace Plyfood.Reposioty.Impl;

public class CartRepositoryImpl : ICartRepository 
{
    private readonly AppDbContext _context;

    public CartRepositoryImpl(AppDbContext context)
    {
        _context = context;
    }

    public void Save(Cart cart)
    {
        _context.Set<Cart>().Add(cart);
    }
}