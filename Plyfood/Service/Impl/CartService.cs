using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto;
using Plyfood.Dto.CartItems;
using Plyfood.Dto.Order;
using Plyfood.Entity;
using Plyfood.Helper.Exception;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public List<CartItemView> ViewCart(Cart cart)
    {
        var list = _context.CartItems
            .Include(x => x.Product)
            .Where(x => x.Cart_Id == cart.Cart_Id)
            .Select(o => new CartItemView()
            {
                CartItemId = o.Cart_Item_Id,
                ProductName = o.Product.Name_Product,
                Quantity = o.Quantity
            }).ToList();
        return list;
    }
    
    public ResponseModel ClearCart(int userId)
    {
        var cart = _context.Carts.Include(x => x.Items)
            .FirstOrDefault(x => x.User_Id == userId);
        if (cart is null)
        {
            return new ResponseModel()
            {
                Message = "Cart not found",
                Status = "400"
            };
        }
        cart.Items = new List<CartItem>();
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Carts.Update(cart);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "clear success",
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

    public OrderViewDto CartToOrder(int userId)
    {
        var cart = _context.Carts
            .Include(x=>x.User)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefault(x => x.User_Id == userId);
        if (cart is null)
        {
            throw new ErrorException("Cart not found", "400");
        }
        double actual = 0;
        double origin = 0;
        List<OrderDetail> orderDetails = new List<OrderDetail>();
        foreach (var c in cart.Items)
        {
            var priceTotal = c.Product.Price * c.Quantity;
            var orderDetail = new OrderDetail()
            {
                Product_Id = c.Product_Id,
                Price_Total = priceTotal,
                Quantity = c.Quantity,
                Create_At = DateTime.Now
            };
            origin = origin + priceTotal;
            double truePrice = (double) (priceTotal * c.Product.Discount)/100;
            actual += truePrice;
            orderDetails.Add(orderDetail);
        }

        var order = new Order()
        {
            Payment_Id = 2,
            User_Id = cart.User_Id,
            Original_Price = origin,
            Actual_Price = origin - actual,
            Full_name = cart.User.User_Name,
            Phone = cart.User.Phone,
            Address = cart.User.Address,
            Order_Status_Id = 1,
            Created = DateTime.Now,
            OrderDetails = orderDetails
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                transaction.CommitAsync();
                ClearCart(userId);
                var orderChange = _context.Orders
                    .Include(x=> x.OrderDetails)
                    .ThenInclude(x=> x.Product)
                    .Select(o => new OrderViewDto()
                    {
                        OrderId = o.Order_Id,
                        User_Id = o.User_Id,
                        ActualPrice = o.Actual_Price,
                        List = o.OrderDetails.Select(
                            o=> new OrderDetailViewDto()
                            {
                                PriceTotal = o.Price_Total,
                                ProductName = o.Product.Name_Product,
                                Quantity = o.Quantity
                            }
                            ).ToList()
                    }
                    ).ToList()
                    .FirstOrDefault(x=> x.OrderId == order.Order_Id);
                
                return orderChange;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                throw new ErrorException("Serve Invalid", "500");

            }
        }
        
        
    }
}