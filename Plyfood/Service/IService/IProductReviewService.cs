using Plyfood.Dto.Reviews;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IProductReviewService
{
    ResponseModel CreateReview(CreateReviewDto dto,string username);
    ResponseModel UpdateReview(UpdateReviewDto dto,string username);
    ResponseModel DeleteReview(int id,string username);
    List<ProductReview> HistoryReviewsByUser(string username);
}