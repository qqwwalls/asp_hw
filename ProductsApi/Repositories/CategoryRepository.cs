using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetParentsAsync(int id)
    {
        var allCategories = await _context.Categories.ToListAsync();
        var parents = new List<Category>();
        
        var current = allCategories.FirstOrDefault(c => c.Id == id);
        while (current != null && current.ParentId.HasValue)
        {
            var parent = allCategories.FirstOrDefault(c => c.Id == current.ParentId.Value);
            if (parent != null)
            {
                parents.Add(parent);
                current = parent;
            }
            else
            {
                break;
            }
        }
        
        return parents;
    }

    public async Task<IEnumerable<Category>> GetChildrenAsync(int id)
    {
        var allCategories = await _context.Categories.ToListAsync();
        var children = new List<Category>();

        void CollectChildren(int parentId)
        {
            var directChildren = allCategories.Where(c => c.ParentId == parentId).ToList();
            children.AddRange(directChildren);
            foreach (var child in directChildren)
            {
                CollectChildren(child.Id);
            }
        }

        CollectChildren(id);
        return children;
    }
}
