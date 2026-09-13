using MediatR;
using ProductsApi.CQRS.Queries;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ProductsApi.CQRS.Handlers;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryReadDto?>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryReadDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetByIdAsync(request.Id);
    }
}
