using AutoMapper;
using Plyfood.Context;
using Plyfood.Dto.ProductTypes;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class ProductTypeService : IProductTypeService
{
    private readonly AppDbContext _context;
    // private readonly IMapper _mapper;
    private readonly Status _status;

    public ProductTypeService(AppDbContext context,Status status)
    {
        _context = context;
        _status = status;
    }

    public ResponseModel Save(ProductTypeCreateForm form)
    {
        ProductType productType = new ProductType()
        {
            Name_Product_Type = form.NameProductType,
            Image_Type_Product = form.Image,
            Create_At = DateTime.Now

        };
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.ProductTypes.Add(productType);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "Add success",
                    Status = _status.Ok
                };
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResponseModel()
                {
                    Message = "add false " + e.Message,
                    Status = "400 BabRequest"

                };
            }
        }
    }

    public ResponseModel Update(ProductTypeUpdateForm form)
    {
        ProductType productType = new ProductType()
        {
            Product_Type_Id = form.ProductTypeId,
            Name_Product_Type = form.NameProductType,
            Image_Type_Product = form.Image,
            Update_At = DateTime.Now
        };
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.ProductTypes.Add(productType);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponseModel()
                {
                    Message = "Update success",
                    Status = "200 Ok"
                };
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResponseModel()
                {
                    Message = "Update false " + e.Message,
                    Status = "400 BabRequest"

                };
            }
        }
    }

    public List<ProductTypeView> GetAll()
    {
        var productTypes = _context.ProductTypes.ToList();
        var productTypeViews = new List<ProductTypeView>();
        foreach (var pt in productTypes)
        {
            var view = new ProductTypeView()
            {
                Product_Type_Id = pt.Product_Type_Id,
                Name_Product_Type = pt.Name_Product_Type,
                Image_Type_Product = pt.Image_Type_Product,
                Create_At = pt.Create_At,
                Update_At = pt.Update_At
            };
            productTypeViews.Add(view);
        }
        return productTypeViews;
    }

    public List<ProductTypeView> FindByName(string name)
    {
        var productTypes = _context.ProductTypes
            .Where(x => x.Name_Product_Type.Contains(name)).ToList();
        var productTypeViews = new List<ProductTypeView>();
        foreach (var pt in productTypes)
        {
            var view = new ProductTypeView()
            {
                Product_Type_Id = pt.Product_Type_Id,
                Name_Product_Type = pt.Name_Product_Type,
                Image_Type_Product = pt.Image_Type_Product,
                Create_At = pt.Create_At,
                Update_At = pt.Update_At
            };
            productTypeViews.Add(view);
        }
        return productTypeViews;
    }

    
    
    
}