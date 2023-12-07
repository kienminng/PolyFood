using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.Reviews;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.Helper.Exception;
using Plyfood.Helper.ResponseMessage;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class ProductReviewService : IProductReviewService
{
    private readonly AppDbContext _context;
    private readonly ProductMessage _productMessage;
    private readonly Status _status;

    public ProductReviewService(AppDbContext context,ProductMessage productMessage,Status status)
    {
        _context = context;
        _productMessage = productMessage;
        _status = status;
    }

    public ResponseModel CreateReview(CreateReviewDto dto,Account account)
    {
        var user = account.Users.FirstOrDefault();

        var orderDetail = _context.OrdersDetail
            .Include(x => x.Order)
            .FirstOrDefault(x => x.Product_Id == dto.ProductId && x.Order.User_Id == user.User_Id && x.Order.Order_Status_Id == 3);
        if (orderDetail is null)
        {
            return new ResponseModel()
            {
                Message = "Only when you buy it can you write a review",
                Status = "400"
            };
        }
        var review = new ProductReview()
        {
            Product_Id = dto.ProductId,
            User = user,
            Content_Seen = dto.ContentSeen,
            Ponit_Evaluation = dto.PointEvaluation
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.ProductReviews.Add(review);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message ="Success",
                    Status = _status.Ok
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(_productMessage.ServeError);
            }
           
        }
    }
    

    public ResponseModel UpdateReview(UpdateReviewDto dto,Account account)
    {
        ProductReview productReview = ValidateProductReview(dto.ProductReviewId, account.Users.FirstOrDefault().User_Id);
        productReview.Content_Seen = dto.ContentSeen;
        productReview.Ponit_Evaluation = dto.PointEvaluation;
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.ProductReviews.Update(productReview);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = _productMessage.AddSuccess,
                    Status = _status.Ok
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                throw new Exception(_productMessage.ServeError);
            }
            
        }
    }

    public ResponseModel DeleteReview(int id,Account account)
    {
        var review = ValidateProductReview(id, account.Users.FirstOrDefault().User_Id);
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.ProductReviews.Remove(review);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = _productMessage.DeleteSuccess,
                    Status = _status.Ok
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(_productMessage.ServeError);
            }
        }
    }

    public List<ProductReview> HistoryReviewsByUser(Account account)
    {
        List<ProductReview> reviews = _context.ProductReviews
            .Where(x => x.User.User_Id == account.Users.FirstOrDefault().User_Id ).ToList();
        return reviews;
    }
    
    
    private ProductReview ValidateProductReview(int reviewId, int userId)
    {
        var review =
            _context.ProductReviews
                .Include(x=> x.User)
                .FirstOrDefault(x => x.Product_Review_Id == reviewId);
        if (review is null)
        {
            throw new ErrorException("Not found exception","400");
        }

        if (review.User.User_Id != userId)
        {  
           throw new ErrorException("UnAuthozation","401");
        }

        return review;
    }
}