using Plyfood.Context;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.Helper.Exception;
using Plyfood.Helper.ResponseMessage;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class OrderStatusService : IOrderStatusService
{
    private readonly AppDbContext _context;
    private readonly OrderMessage _orderMessage;
    private readonly Status _status;

    public OrderStatusService(AppDbContext context, OrderMessage orderMessage,Status status)
    {
        _context = context;
        _orderMessage = orderMessage;
        _status = status;

    }

    public ResponseModel Create(string statusName)
    {
        var check = IsExist(statusName);
        if (check != null)
        {
            return new ResponseModel()
            {
                Message = _orderMessage.OrderFalse,
                Status = _status.BadRequest
            };
        }
        var orderStatus = new OrderStatus()
        {
            Status_Name = statusName
        };
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.OrderStatuses.Add(orderStatus);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = _orderMessage.OrderSuccess,
                    Status = _status.Ok
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

    public ResponseModel Update(OrderStatus orderStatus)
    {
        var check = _context.OrderStatuses.FirstOrDefault(x =>
            x.Status_Name.Equals(orderStatus) && x.Order_Status_Id != orderStatus.Order_Status_Id);
        if (check!= null)
        {
            return new ResponseModel()
            {
                Message = "Your order type name was existed",
                Status = _status.BadRequest
            };
        }

        _context.OrderStatuses.Update(orderStatus);
        _context.SaveChanges();
        return new ResponseModel()
        {
            Message = _orderMessage.OrderSuccess,
            Status = _status.Ok
        };
    }

    public List<OrderStatus> GetAll()
    {
        var list = _context.OrderStatuses.ToList();
        return list;
    }

    public ResponseModel Delete(int id)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            var status = _context.OrderStatuses.FirstOrDefault(x => x.Order_Status_Id == id);
            if (status is null)
            {
                return new ResponseModel()
                {
                    Message = "Not exist in program",
                    Status = _status.NotFound
                };
            }

            try
            {
                _context.OrderStatuses.Remove(status);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "delete success",
                    Status = _status.Ok
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

    private OrderStatus? IsExist(string name)
    {
        var check = _context.OrderStatuses.FirstOrDefault(x => x.Status_Name.Equals(name));
        return check;
    }
}