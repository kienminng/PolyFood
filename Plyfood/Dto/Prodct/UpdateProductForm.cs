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
    public DateTime? Create_At { get; set; }
}