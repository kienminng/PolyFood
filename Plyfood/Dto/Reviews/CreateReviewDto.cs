using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.Reviews;

public class CreateReviewDto
{
    [Required]
    public int ProductId { get; set; }
    public string? ContentSeen { get; set; }
    [Range(1,10)]
    public int PointEvaluation { get; set; }
}