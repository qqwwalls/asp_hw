using ProductsApi.Models;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface IProductService
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    IEnumerable<Product> SearchByName(string name);
    Product Create(ProductDto dto);
    Product? Update(int id, ProductDto dto);
    bool Delete(int id);
}
