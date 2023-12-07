using Plyfood.Dto.Order;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IOrderService
{
    ResponseModel Create(OrderCreatingForm creatingForm,int userId);
    ResponseModel Update(OrderUpdateForm updateForm);
    List<OrderViewDto>? GetAllLoginUser(string username);

    ResponseModel ChangeStatus(int orderId, int orderStatusId);

    ListOrderAndMoneyTotal CalculateRevenueByTimePeriod(DateTime startDate, DateTime endDate);
}