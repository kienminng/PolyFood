using Plyfood.Dto.CartItems;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface ICartItemService
{
    ResponseModel Save(CartItem cartItem);
    ResponseModel Update(UpdateCartItemDto cartItemDto);
    ResponseModel Delete(int id);
}