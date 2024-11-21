using Bar.Domain;
using Bar.Domain.Entities;
using Bar.Domain.Models;
namespace Bar.API.Services
{
    public interface IProductService
    {
        Task<ResponseData<ListModel<Product>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3);
        Task<ResponseData<Product>> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, Product product);
        Task DeleteProductAsync(int id);
        Task<ResponseData<Product>> CreateProductAsync(Product product);
        Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile);
    }
}
