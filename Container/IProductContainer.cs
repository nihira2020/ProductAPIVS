using ProductAPIVS.Models;
namespace ProductAPIVS.Container;
public interface IProductContainer
{
    Task<List<Product>> GetAll();
    Task<Product> GetbyCode(int code);
    Task<bool> Remove(int code);
    Task<bool> Save(Product _product);
}