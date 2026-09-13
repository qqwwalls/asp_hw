using MediatR;
using ProductsApi.CQRS.Commands;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ProductsApi.CQRS.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductReadDto>
{
    private readonly IProductService _productService;

    public CreateProductHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ProductReadDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.CreateAsync(request.Dto);
    }
}
