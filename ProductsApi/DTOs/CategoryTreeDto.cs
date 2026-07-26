using System.Collections.Generic;

namespace ProductsApi.DTOs;

public class CategoryTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new List<CategoryTreeDto>();
}
