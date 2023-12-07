using Plyfood.Dto.Reviews;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IProductReviewService
{
    ResponseModel CreateReview(CreateReviewDto dto,Account account);
    ResponseModel UpdateReview(UpdateReviewDto dto,Account account);
    ResponseModel DeleteReview(int id,Account account);
    List<ProductReview> HistoryReviewsByUser(Account account);
}