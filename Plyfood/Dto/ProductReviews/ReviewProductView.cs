namespace Plyfood.Dto.ProductReviews;

public class ReviewProductView
{
    public string ProductName { get; set; }
    public string Username { get; set; }
    public string Content_rated { get; set; }
    public int? Ponit_Evaluation { get; set; }
    public string Content_Seen { get; set; }
    public int Status { get; set; } 
    public DateTime? Create_At { get; set; } 
    public DateTime? Update_At { get; set; }
}