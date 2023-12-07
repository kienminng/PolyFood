using Plyfood.Context;
using Plyfood.Dto.CartItems;
using Plyfood.Entity;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class CartItemService : ICartItemService
{
    private readonly AppDbContext _context;

    public CartItemService(AppDbContext context)
    {
        _context = context;
    }

    public ResponseModel Save(CartItem cartItem)
    {
        var check = ValidateItemInfo(cartItem.Product_Id, cartItem.Cart_Id);
        if (check is null)
        {
            return SaveChange(cartItem,true);
        }
        check.Quantity = check.Quantity + cartItem.Quantity;
        return SaveChange(check,false);
    }

    public ResponseModel Update(UpdateCartItemDto cartItemDto)
    {
        var cartItem = FindById(cartItemDto.CartItemId);
        if (cartItem is null)
        {
            return new ResponseModel()
            {
                Message = "CartItem Not found",
                Status = "400"
            };
        }

        return SaveChange(cartItem,false);
    }

    public ResponseModel Delete(int id)
    {
        var cartItem = FindById(id);
        if (cartItem is null)
        {
            return new ResponseModel()
            {
                Message = "CartItem Not found",
                Status = "400"
            };
        }

        _context.CartItems.Remove(cartItem);
        _context.SaveChanges();
        return new ResponseModel()
        {
            Message = "delete success",
            Status = "200"
        };
    }

    private ResponseModel SaveChange(CartItem cartItem,bool check)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                if (check)
                {
                   _context.CartItems.Add(cartItem); 
                }

                if (!check)
                {
                    _context.CartItems.Update(cartItem);
                }
                
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "success",
                    Status = "200"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return new ResponseModel()
                {
                    Message = "Serve Invalid",
                    Status = "500"
                };
            }
        }
    }

    private CartItem FindById(int id)
    {
        return _context.CartItems.FirstOrDefault(x => x.Cart_Item_Id == id);
    }

    private CartItem ValidateItemInfo(int? productId, int cartId)
    {
        var item = _context.CartItems.FirstOrDefault(x => x.Product_Id == productId && x.Cart_Id == cartId);
        return item;
    }
}