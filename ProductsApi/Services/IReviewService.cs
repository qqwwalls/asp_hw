using ProductsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public interface IReviewService
{
    Task AddReviewAsync(ProductReview review);
    Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId);
}
