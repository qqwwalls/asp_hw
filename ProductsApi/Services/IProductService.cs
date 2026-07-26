using System.Collections.Generic;
using System.Threading.Tasks;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface IProductService
{
    Task<ProductReadDto> CreateAsync(ProductCreateDto dto);
    Task<IEnumerable<ProductReadDto>> GetAllAsync();
    Task<ProductReadDto?> GetByIdAsync(int id);
}
