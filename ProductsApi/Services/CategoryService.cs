using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Url = dto.Url,
            ParentId = dto.ParentId,
            IsActive = true
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Url = category.Url,
            IsActive = category.IsActive,
            ParentId = category.ParentId
        };
    }

    public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Url = c.Url,
                IsActive = c.IsActive,
                ParentId = c.ParentId
            })
            .ToListAsync();
    }

    public async Task<CategoryReadDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return null;

        return new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Url = category.Url,
            IsActive = category.IsActive,
            ParentId = category.ParentId
        };
    }
}
