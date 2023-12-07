using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.Prodct;
using Plyfood.Dto.ProductReviews;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.Helper.Exception;
using Plyfood.Helper.ResponseMessage;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    // private readonly IMapper _mapper;
    private readonly ProductMessage _productMessage;
    private readonly Status _status;

    public ProductService(AppDbContext appDbContext, ProductMessage productMessage, Status status)
    {
        _context = appDbContext;
        _productMessage = productMessage;
        _status = status;
    }

    public ResponseModel CreateProduct(CreateProductForm form)
    {
        if (!ValidateProductType(form.ProductTypeId))
        {
            return new ResponseModel()
            {
                Message = _productMessage.ProductTypeNull,
                Status = _status.BadRequest
            };
        }

        if (!ValidateNameAndImage(form.NameProduct, form.AvatarImageProduct))
        {
            return new ResponseModel()
            {
                Message = _productMessage.NameAndImageValid,
                Status = _status.BadRequest
            };
        }

        Product product = DataTranfFormCreateFormToProduct(form);
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Products.Add(product);
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
                return new ResponseModel()
                {
                    Message = _productMessage.ProductInvalid + " " + e.Message,
                    Status = _status.BadRequest
                };
            }
        }
    }

    public ResponseModel UpdateProduct(UpdateProductForm form)
    {
        Product product = UpdateFormToProduct(form);
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Products.Update(product);
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
                transaction.Rollback();
                throw new ErrorException(_productMessage.ProductInvalid+ " " + e.Message);
            }
        }
    }

    public List<ProductView> FindByName(string name)
    {
        var list = _context.Products.Where(x => x.Name_Product.Contains(name) && x.Status == 1).ToList();
        var views = DataTranferListProductToViews(list);
        return views;
    }

    public ProductView FindById(int id)
    {
        var product = _context.Products
            .Include(x=> x.ProductType)
            .Include(x=> x.Reviews)
            .ThenInclude(x=> x.User)
            .Select(o => TranferProductToView(o))
            .FirstOrDefault(x => x.Product_Id == id)
            ;
        if (product is null)
        {
            throw new Exception(_productMessage.ProductIsNotExist);
        }

        return product;
    }

    public ResponseModel ChangeStatus(int status)
    {
        throw new NotImplementedException();
    }

    public bool ViewProduct(int productId)
    {
        throw new NotImplementedException();
    }

    public ProductView ProductView(int id)
    {
        var check = _context.Products
            .Include(x=>x.ProductType)
            .Include(x=> x.OrderDetails)
            .Include(x=>x.Reviews).ThenInclude(x=> x.User)
            .FirstOrDefault(x => x.Product_Id == id);
        if (check != null)
        {
            check.Number_Of_View = check.Number_Of_View + 1;

            _context.Products.Update(check);
            _context.SaveChanges();
            var view = TranferProductToView(check);
            return view;
        }
        throw new Exception(_productMessage.ProductInvalid);
    }

    private Product DataTranfFormCreateFormToProduct(CreateProductForm form)
    {
        Product product = new Product()
        {
            Name_Product = form.NameProduct,
            Number_Of_View = 0,
            Avatar_Image_Product = form.AvatarImageProduct,
            Status = 1,
            Discount = form.Discount,
            Price = form.Price,
            Title = form.Title,
            ProductType_Id = form.ProductTypeId,
            Create_At = DateTime.Now,
        };
        return product;
    }

    private bool ValidateProductType(int? id)
    {
        var check = _context.ProductTypes.Find(id);
        if (check is null)
        {
            return false;
        }

        return true;
    }

    private bool ValidateNameAndImage(string? name, string? image)
    {
        if (name is null || image is null)
        {
            throw new Exception(_productMessage.NameAndImageValid);
        }

        var check = _context.Products.FirstOrDefault(x => x.Name_Product.Equals(name));
        if (check != null)
        {
            return false;
        }

        return true;
    }

    private ProductView TranferProductToView(Product o)
    {
        ProductView productView = new ProductView()
        {
            Product_Id = o.Product_Id,
            ProductTypeName = o.ProductType.Name_Product_Type,
            Name_Product = o.Name_Product,
            Price = o.Price,
            Avatar_Image_Product = o.Avatar_Image_Product,
            Title = o.Title,
            Discount = o.Discount,
            Status = o.Status,
            Number_Of_View = o.Number_Of_View,
            Reviews = o.Reviews.Select( 
                o=> new ReviewProductView()
                {
                    Content_rated = o.Content_rated,
                    Content_Seen = o.Content_Seen,
                    Ponit_Evaluation = o.Ponit_Evaluation,
                    Status = o.Status,
                    Username = o.User.User_Name,
                    Create_At = o.Create_At,
                    Update_At = o.Update_At
                }).ToList(),
            PointAvg = o.Reviews.Average(x=> x.Ponit_Evaluation),
            Create_At = o.Create_At,
            Update_At = o.Update_At
        };
        return productView;
    }

    private Product UpdateFormToProduct(UpdateProductForm uf)
    {
        Product product = new Product()
        {
            Product_Id = uf.ProductId,
            Name_Product = uf.NameProduct,
            Price = uf.Price,
            Avatar_Image_Product = uf.Avatar,
            Title = uf.Title,
            Status = uf.Status,
            Discount = uf.Discount,
        };
        return product;
    }

    private List<ProductView> DataTranferListProductToViews(List<Product> products)
    {
        List<ProductView> views = new List<ProductView>();
        if (products != null)
        {
            
            foreach (var product in products)
            {
                ProductView productView = TranferProductToView(product);
                views.Add(productView);
            }

            return views;
        }
        return views;
    }
    
}