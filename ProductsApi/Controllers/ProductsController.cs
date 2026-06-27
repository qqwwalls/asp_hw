using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.DTOs;
using ProductsApi.Services;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        return Ok(_productService.GetAll());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetProduct(int id)
    {
        var product = _productService.GetById(id);
        if (product == null) return NotFound("Product not found");
        return Ok(product);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Search([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Name parameter is required");

        return Ok(_productService.SearchByName(name));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _productService.Create(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = _productService.Update(id, dto);
        if (updated == null) return NotFound("Product not found");

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (!_productService.Delete(id))
            return NotFound("Product not found");

        return NoContent();
    }
}
