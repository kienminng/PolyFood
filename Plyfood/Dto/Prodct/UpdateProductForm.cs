using Plyfood.Entity;

namespace Plyfood.Dto.Prodct;

public class UpdateProductForm
{
    public int ProductId { get; set; }
    public string NameProduct { get; set; }
    public double Price { get; set; }
    public string Avatar { get; set; }
    public string Title { get; set; }
    public int Discount { get; set; }
    public int Status { get; set; }
    
    public Product UpdateFormToProduct(UpdateProductForm uf)
    {
        Product product = new Product()
        {
            Product_Id = uf.ProductId,
            Name_Product = uf.NameProduct,
            Price = uf.Price,
            Avatar_Image_Product = uf.Avatar,
            Title = uf.Title,
            Status = uf.Status,
            Discount = uf.Discount,
        };
        return product;
    }
    
    
}