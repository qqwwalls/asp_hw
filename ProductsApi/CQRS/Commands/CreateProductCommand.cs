using MediatR;
using ProductsApi.DTOs;

namespace ProductsApi.CQRS.Commands;

public class CreateProductCommand : IRequest<ProductReadDto>
{
    public ProductCreateDto Dto { get; set; }

    public CreateProductCommand(ProductCreateDto dto)
    {
        Dto = dto;
    }
}
