using Plyfood.Dto.CartItems;
using Plyfood.Dto.Order;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface ICartService
{
    List<CartItemView> ViewCart(Cart cart);
    ResponseModel ClearCart(int userId);

    OrderViewDto CartToOrder(int userId);
}