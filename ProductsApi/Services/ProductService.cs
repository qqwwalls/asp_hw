using ProductsApi.Models;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public class ProductService : IProductService
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Phone", Price = 499.99m },
        new Product { Id = 3, Name = "Headphones", Price = 99.99m }
    };

    public IEnumerable<Product> GetAll() => _products;

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public IEnumerable<Product> SearchByName(string name)
    {
        return _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public Product Create(ProductDto dto)
    {
        var newProduct = new Product
        {
            Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1,
            Name = dto.Name,
            Price = dto.Price
        };
        _products.Add(newProduct);
        return newProduct;
    }

    public Product? Update(int id, ProductDto dto)
    {
        var product = GetById(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Price = dto.Price;
        return product;
    }

    public bool Delete(int id)
    {
        var product = GetById(id);
        if (product == null) return false;

        _products.Remove(product);
        return true;
    }
}
