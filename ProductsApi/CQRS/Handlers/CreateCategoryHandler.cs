using MediatR;
using ProductsApi.CQRS.Commands;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ProductsApi.CQRS.Handlers;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryReadDto>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryReadDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryService.CreateAsync(request.Dto);
    }
}
