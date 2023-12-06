using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IOrderStatusService
{
    ResponseModel Create(string statusName);
    ResponseModel Update(OrderStatus orderStatus);
    List<OrderStatus> GetAll();
    ResponseModel Delete(int id);
}