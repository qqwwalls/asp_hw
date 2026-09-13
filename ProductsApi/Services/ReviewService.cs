using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class ReviewService : IReviewService
{
    private readonly IMongoCollection<ProductReview> _reviews;

    public ReviewService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDb:DatabaseName"]);
        _reviews = database.GetCollection<ProductReview>("Reviews");
    }

    public async Task AddReviewAsync(ProductReview review)
    {
        await _reviews.InsertOneAsync(review);
    }

    public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
    {
        return await _reviews.Find(r => r.ProductId == productId).ToListAsync();
    }
}
