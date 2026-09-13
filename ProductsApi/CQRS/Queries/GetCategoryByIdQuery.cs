using MediatR;
using ProductsApi.DTOs;

namespace ProductsApi.CQRS.Queries;

public class GetCategoryByIdQuery : IRequest<CategoryReadDto?>
{
    public int Id { get; set; }

    public GetCategoryByIdQuery(int id)
    {
        Id = id;
    }
}
