namespace Plyfood.Dto.ProductTypes;

public class ProductTypeView
{
    public int Product_Type_Id { get; set; }
    public string Name_Product_Type { get; set; } 
    public string Image_Type_Product {  get; set; } 
    public DateTime Create_At { get; set; } 
    public DateTime? Update_At { get; set; }

}