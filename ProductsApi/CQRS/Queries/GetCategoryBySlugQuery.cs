using MediatR;
using ProductsApi.DTOs;

namespace ProductsApi.CQRS.Queries;

public class GetCategoryBySlugQuery : IRequest<CategoryReadDto?>
{
    public string Slug { get; set; }

    public GetCategoryBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
