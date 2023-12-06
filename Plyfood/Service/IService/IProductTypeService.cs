using Plyfood.Dto.ProductTypes;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IProductTypeService
{
    ResponseModel Save(ProductTypeCreateForm form);
    ResponseModel Update(ProductTypeUpdateForm form);
    List<ProductTypeView> GetAll();
    List<ProductTypeView> FindByName(string name);
    
}