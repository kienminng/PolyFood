using Plyfood.Context;
using Plyfood.Entity;
using Plyfood.Helper.Exception;

namespace Plyfood.Dto;

public class OrderDetailCreatingDto
{

    public int? Product_Id { get; set; }
    public int Quantity { get; set; } = 0;
    
    
}