using MediatR;
using ProductsApi.DTOs;

namespace ProductsApi.CQRS.Commands;

public class CreateCategoryCommand : IRequest<CategoryReadDto>
{
    public CategoryCreateDto Dto { get; set; }

    public CreateCategoryCommand(CategoryCreateDto dto)
    {
        Dto = dto;
    }
}
