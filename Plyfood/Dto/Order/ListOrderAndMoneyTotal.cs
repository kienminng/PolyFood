namespace Plyfood.Dto.Order;

public class ListOrderAndMoneyTotal
{
    public List<OrderViewDto> List { get; set; }
    public double Money { get; set; }
}