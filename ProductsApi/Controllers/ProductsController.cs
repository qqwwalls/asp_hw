using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductsApi.CQRS.Commands;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;
using ProductsApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;
    private readonly IMediator _mediator;

    public ProductsController(IProductService productService, IReviewService reviewService, IMediator mediator)
    {
        _productService = productService;
        _reviewService = reviewService;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _mediator.Send(new CreateProductCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        
        return Ok(product);
    }

    [HttpPost("{id}/reviews")]
    public async Task<IActionResult> AddReview(int id, [FromBody] CreateReviewDto dto)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        var review = new ProductReview
        {
            ProductId = id,
            Author = dto.Author,
            Text = dto.Text,
            IsQuestion = dto.IsQuestion,
            CreatedAt = System.DateTime.UtcNow
        };

        await _reviewService.AddReviewAsync(review);

        return Ok(new { message = "Review added successfully" });
    }
}
