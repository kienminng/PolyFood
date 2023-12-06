using Plyfood.Entity;

namespace Plyfood.Dto.CartItems;

public class CreatingCartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public CartItem ChangeToCart()
    {
        var cartItem = new CartItem()
        {
            Product_Id = this.ProductId,
            Quantity = this.Quantity
        };
        return cartItem;
    }
}