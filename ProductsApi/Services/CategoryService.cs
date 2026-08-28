using Microsoft.Extensions.Caching.Distributed;
using ProductsApi.DTOs;
using ProductsApi.Models;
using ProductsApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IDistributedCache _cache;

    public CategoryService(ICategoryRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            ParentId = dto.ParentId
        };

        var created = await _repository.AddAsync(category);

        return new CategoryReadDto
        {
            Id = created.Id,
            Name = created.Name
        };
    }

    public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => new CategoryReadDto
        {
            Id = c.Id,
            Name = c.Name
        });
    }

    public async Task<CategoryReadDto?> GetByIdAsync(int id)
    {
        string cacheKey = $"category_{id}";
        var cachedCategory = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCategory))
        {
            return JsonSerializer.Deserialize<CategoryReadDto>(cachedCategory);
        }

        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        var dto = new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name
        };

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

        return dto;
    }

    public async Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        category.Name = dto.Name;
        
        var updated = await _repository.UpdateAsync(category);

        await _cache.RemoveAsync($"category_{id}");

        return new CategoryReadDto
        {
            Id = updated.Id,
            Name = updated.Name
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await _cache.RemoveAsync($"category_{id}");
        }
        return result;
    }

    public async Task<IEnumerable<CategoryReadDto>> GetParentsAsync(int id)
    {
        var parents = await _repository.GetParentsAsync(id);
        return parents.Select(p => new CategoryReadDto
        {
            Id = p.Id,
            Name = p.Name
        });
    }

    public async Task<IEnumerable<CategoryReadDto>> GetChildrenAsync(int id)
    {
        var children = await _repository.GetChildrenAsync(id);
        return children.Select(c => new CategoryReadDto
        {
            Id = c.Id,
            Name = c.Name
        });
    }

    public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync()
    {
        var allCategories = await _repository.GetAllAsync();
        
        var lookup = allCategories.ToDictionary(
            c => c.Id, 
            c => new CategoryTreeDto { Id = c.Id, Name = c.Name, ParentId = c.ParentId }
        );

        var tree = new List<CategoryTreeDto>();

        foreach (var category in lookup.Values)
        {
            if (category.ParentId.HasValue && lookup.TryGetValue(category.ParentId.Value, out var parent))
            {
                parent.Children.Add(category);
            }
            else
            {
                tree.Add(category);
            }
        }

        return tree;
    }
}
