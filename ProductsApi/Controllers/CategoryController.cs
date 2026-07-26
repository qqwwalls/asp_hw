using Microsoft.AspNetCore.Mvc;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryReadDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryReadDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _categoryService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }

    [HttpGet("{id:int}/parents")]
    [ProducesResponseType(typeof(IEnumerable<CategoryReadDto>), 200)]
    public async Task<IActionResult> GetParents(int id)
    {
        var parents = await _categoryService.GetParentsAsync(id);
        return Ok(parents);
    }

    [HttpGet("{id:int}/children")]
    [ProducesResponseType(typeof(IEnumerable<CategoryReadDto>), 200)]
    public async Task<IActionResult> GetChildren(int id)
    {
        var children = await _categoryService.GetChildrenAsync(id);
        return Ok(children);
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(IEnumerable<CategoryTreeDto>), 200)]
    public async Task<IActionResult> GetTree()
    {
        var tree = await _categoryService.GetTreeAsync();
        return Ok(tree);
    }
}
