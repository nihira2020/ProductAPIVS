
using Microsoft.EntityFrameworkCore;
using ProductAPIVS.Models;

namespace ProductAPIVS.Container;
public class ProductContainer :IProductContainer
{
    private readonly Learn_DBContext _DBContext;
    public ProductContainer(Learn_DBContext dBContext)
    {
        this._DBContext = dBContext;
    }

    public async Task<List<Product>> GetAll()
    {
        return await _DBContext.Products.ToListAsync();
    }
    public async Task<Product> GetbyCode(int code)
    {
        var product = await _DBContext.Products.FindAsync(code);
        if (product != null)
        {
            return product;
        }
        else
        {
            return new Product();
        }
    }
    public async Task<bool> Remove(int code)
    {
        var product = await _DBContext.Products.FindAsync(code);
        if (product != null)
        {
            this._DBContext.Remove(product);
            await this._DBContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> Save(Product _product)
    {
        var product = this._DBContext.Products.FirstOrDefault(o => o.Id == _product.Id);
            if (product != null)
            {
                product.Name = _product.Name;
                product.Price = _product.Price;
                await this._DBContext.SaveChangesAsync();
            }
            else
            {
                this._DBContext.Products.Add(_product);
                await this._DBContext.SaveChangesAsync();
            }
            return true;
    }



}