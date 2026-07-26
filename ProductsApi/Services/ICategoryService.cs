using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface ICategoryService
{
    CategoryReadDto Create(CategoryCreateDto dto);
    IEnumerable<CategoryReadDto> GetAll();
    CategoryReadDto? GetById(int id);
}
