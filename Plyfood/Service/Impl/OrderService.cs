using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto;
using Plyfood.Dto.Order;
using Plyfood.Entity;
using Plyfood.Helper.Exception;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public ResponseModel Create(OrderCreatingForm creatingForm, int UserId)
    {
        var order = ChangeToOrder(creatingForm, UserId);
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "Create Order success",
                    Status = "200"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return new ResponseModel()
                {
                    Message = "Lỗi dữ liệu dường truyền",
                    Status = "500"
                };
            }
        }
    }
    

    public ResponseModel Update(OrderUpdateForm updateForm)
    {
        throw new NotImplementedException();
    }

    public ResponseModel ChangeStatus(int orderId, int orderStatusId)
    {
        var order = _context.Orders.FirstOrDefault(x => x.Order_Id == orderId);
        if (order is null)
        {
            return new ResponseModel()
            {
                Message = "order not found ",
                Status = "500"
            };
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                order.Order_Status_Id = orderStatusId;
                _context.Orders.Update(order);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "update status success",
                    Status = "200"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return new ResponseModel()
                {
                    Message = "Serve error",
                    Status = "500"
                };
            }
        }
    }
    
    public ListOrderAndMoneyTotal CalculateRevenueByTimePeriod(DateTime startDate, DateTime endDate)
    {
        var ordersWithTotal = _context.Orders
            .Where(o => o.Created >= startDate && o.Created <= endDate  && o.Order_Status_Id == 3)
            
            .Select(o => new OrderViewDto()
            {
                OrderId = o.Order_Id,
                ActualPrice = o.Actual_Price,
                CreateAt = o.Created ?? DateTime.MinValue, // Ensure non-nullable DateTime
                OrderStatus = o.Order_Status.Status_Name,
                List = o.OrderDetails.Select(
                    o => new OrderDetailViewDto()
                    {
                        OrderDetailId = o.Order_Detail_Id,
                        ProductName = o.Product.Name_Product,
                        Quantity = o.Quantity,
                        PriceTotal = o.Price_Total,
                        CreateAt = o.Create_At
                    })
                    .ToList(),
                User_Id = o.User_Id
            }).ToList();

        var totalRevenue = ordersWithTotal.Sum(o => o.ActualPrice);
        var ListOrderAndMoney = new ListOrderAndMoneyTotal()
        {
            List = ordersWithTotal,
            Money = totalRevenue
        };

        return ListOrderAndMoney;
    }

    public List<OrderViewDto>? GetAllLoginUser(string username)
    {
        var list = _context.Orders
            .Include(x => x.User)
            .Include(x => x.Order_Status)
            .Include(x => x.OrderDetails)
            .ThenInclude(x=> x.Product)
            .Where(x => x.User.User_Name == username && x.Order_Status_Id != 4)
            .Select(o => new OrderViewDto()
            {
                OrderId = o.Order_Id,
                User_Id = o.User_Id,
                ActualPrice = o.Actual_Price,
                OrderStatus = o.Order_Status.Status_Name,
                List = o.OrderDetails.Select(
                    o => new OrderDetailViewDto()
                    {
                        OrderDetailId = o.Order_Detail_Id,
                        ProductName = o.Product.Name_Product,
                        Quantity = o.Quantity,
                        PriceTotal = o.Price_Total,
                        CreateAt = o.Create_At
                    }).ToList()

            }).ToList();
        return list;
    }

    public Order ChangeToOrder(OrderCreatingForm creatingForm, int UserId)
    {
        double actual = 0;
        double origin = 0;
        List<OrderDetail> orderDetails = new List<OrderDetail>();
        foreach (var s in creatingForm.OrderDetails)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                Product_Id = s.Product_Id,
                Quantity = s.Quantity,
                Create_At = DateTime.Now
            };
            var product = _context.Products
                .FirstOrDefault(x => x.Product_Id == s.Product_Id);
            if (product is null)
            {
                throw new ErrorException("Product not found in data", "400");
            }
            orderDetail.Price_Total = product.Price * orderDetail.Quantity;
            origin = origin + orderDetail.Price_Total;
            double truePrice = orderDetail.Price_Total;
            if (product.Discount != 0 )
            {
                 truePrice = (double)(orderDetail.Price_Total*product.Discount)/100; 
            }
           
            actual = actual + truePrice;
            orderDetails.Add(orderDetail);
            
        }

        var order = new Order()
        {
            User_Id = UserId,
            Payment_Id = 2,
            Full_name = creatingForm.FullName,
            Phone = creatingForm.PhoneNumber,
            Address = creatingForm.Address,
            Order_Status_Id = 5,
            Created = DateTime.Now,
            Original_Price = origin,
            Actual_Price = actual,
            OrderDetails = orderDetails
        };

        return order;
    }
    
}