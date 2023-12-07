namespace Plyfood.Dto.Order;

public class OrderViewDto
{
    public int OrderId { get; set; }
    public int? User_Id { get; set; }
    public double ActualPrice { get; set; }
    public string OrderStatus { get; set; }
    public List<OrderDetailViewDto> List { get; set; }
    public DateTime CreateAt { get; set; }
}