using System.Collections.Generic;
using System.Threading.Tasks;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface ICategoryService
{
    Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto);
    Task<IEnumerable<CategoryReadDto>> GetAllAsync();
    Task<CategoryReadDto?> GetByIdAsync(int id);
    Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CategoryReadDto>> GetParentsAsync(int id);
    Task<IEnumerable<CategoryReadDto>> GetChildrenAsync(int id);
    Task<IEnumerable<CategoryTreeDto>> GetTreeAsync();
}
