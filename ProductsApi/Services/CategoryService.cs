using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductsApi.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public CategoryReadDto Create(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        _context.Categories.Add(category);
        _context.SaveChanges();

        return new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public IEnumerable<CategoryReadDto> GetAll()
    {
        return _context.Categories
            .Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToList();
    }

    public CategoryReadDto? GetById(int id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return null;

        return new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
