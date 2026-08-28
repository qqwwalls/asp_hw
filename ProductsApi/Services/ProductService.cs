using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using ProductsApi.DTOs;
using ProductsApi.Models;
using ProductsApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _cache;

    public ProductService(IProductRepository repository, IFileService fileService, IConfiguration configuration, IDistributedCache cache)
    {
        _repository = repository;
        _fileService = fileService;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
    {
        var maxImages = _configuration.GetValue<int>("ProductSettings:MaxImages", 5);

        if (dto.Images != null && dto.Images.Count > maxImages)
        {
            throw new InvalidOperationException($"Cannot upload more than {maxImages} images.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price
        };

        if (dto.Images != null)
        {
            foreach (var file in dto.Images)
            {
                var url = await _fileService.SaveFileAsync(file, "products");
                if (!string.IsNullOrEmpty(url))
                {
                    product.Images.Add(new ProductImage { Url = url });
                }
            }
        }

        var created = await _repository.AddAsync(product);

        // Invalidate the cache for all products when a new product is added
        await _cache.RemoveAsync("products_all");

        return new ProductReadDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price,
            ImageUrls = created.Images.Select(i => i.Url).ToList()
        };
    }

    public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
    {
        string cacheKey = "products_all";
        var cachedProducts = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<IEnumerable<ProductReadDto>>(cachedProducts)!;
        }

        var products = await _repository.GetAllAsync();
        var dtos = products.Select(p => new ProductReadDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrls = p.Images.Select(i => i.Url).ToList()
        }).ToList();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos), cacheOptions);

        return dtos;
    }

    public async Task<ProductReadDto?> GetByIdAsync(int id)
    {
        string cacheKey = $"product_{id}";
        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<ProductReadDto>(cachedProduct);
        }

        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        var dto = new ProductReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrls = product.Images.Select(i => i.Url).ToList()
        };

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

        return dto;
    }
}
