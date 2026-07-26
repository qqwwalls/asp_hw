using System.Collections.Generic;
using System.Threading.Tasks;
using ProductsApi.Models;

namespace ProductsApi.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Category>> GetParentsAsync(int id);
    Task<IEnumerable<Category>> GetChildrenAsync(int id);
}
