namespace Plyfood.Dto;

public class OrderDetailViewDto
{
    public int OrderDetailId { get; set; }
    public string ProductName { get; set; }
    public double PriceTotal { get; set; }
    public int Quantity { get; set; }
    public DateTime? CreateAt { get; set; }
}