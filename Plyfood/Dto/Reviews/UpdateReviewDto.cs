using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.Reviews;

public class UpdateReviewDto
{
    [Required]
    public int ProductReviewId { get; set; }
    public string? ContentSeen { get; set; }
    [Range(1,10)]
    public int PointEvaluation { get; set; }
}