using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface ICartService
{
    Cart ViewCart(int userId);
    ResponseModel ClearCart(int userId);

    Order CartToOrder(int userId);
}