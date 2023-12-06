using Plyfood.Dto.Prodct;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IProductService
{
    ResponseModel CreateProduct(CreateProductForm form);
    ResponseModel UpdateProduct(UpdateProductForm form);
    List<ProductView> FindByName(string name);
    Product FindById(int id);
    ResponseModel ChangeStatus(int status);
    bool ViewProduct(int productId);

    ProductView ProductView(int id);

}