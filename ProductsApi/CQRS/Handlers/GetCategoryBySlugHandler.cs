using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsApi.CQRS.Queries;
using ProductsApi.Data;
using ProductsApi.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace ProductsApi.CQRS.Handlers;

public class GetCategoryBySlugHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryReadDto?>
{
    private readonly AppDbContext _context;

    public GetCategoryBySlugHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryReadDto?> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == request.Slug, cancellationToken);
        if (category == null) return null;

        return new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId
        };
    }
}
