using ItemsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItemsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<Item> _items = new()
    {
        new Item { Id = 1, Name = "Laptop" },
        new Item { Id = 2, Name = "Phone" },
        new Item { Id = 3, Name = "Pen" }
    };

    // GET: api/items
    [HttpGet]
    public IActionResult GetItems()
    {
        return Ok(_items);
    }

    // GET: api/items/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetItem(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound("Item not found");
        }
        return Ok(item);
    }

    // GET: api/items/search
    [HttpGet("search")]
    public IActionResult SearchItems([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("The 'name' parameter is required.");
        }

        var results = _items
            .Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(results);
    }

    // POST: api/items
    [HttpPost]
    public IActionResult CreateItem([FromBody] Item newItem)
    {
        if (string.IsNullOrWhiteSpace(newItem.Name))
        {
            return BadRequest("Name cannot be empty.");
        }

        newItem.Id = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1;
        _items.Add(newItem);

        return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
    }

    // PUT: api/items/{id}
    [HttpPut("{id:int}")]
    public IActionResult UpdateItem(int id, [FromBody] Item updatedItem)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound("Item not found");
        }

        if (string.IsNullOrWhiteSpace(updatedItem.Name))
        {
            return BadRequest("Name cannot be empty.");
        }

        item.Name = updatedItem.Name;
        return Ok(item);
    }

    // DELETE: api/items/{id}
    [HttpDelete("{id:int}")]
    public IActionResult DeleteItem(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound("Item not found");
        }

        _items.Remove(item);
        return NoContent();
    }
}
