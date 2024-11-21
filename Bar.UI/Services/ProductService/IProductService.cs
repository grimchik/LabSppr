using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bar.Domain.Entities;
using Bar.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Bar.UI.Services.ProductService
{
    public interface IProductService
    {
        Task<ResponseData<ListModel<Product>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3);
        Task<ResponseData<Product>> GetProductByIdAsync(int id);
        Task<ResponseData<Product>> CreateProductAsync(Product product,IFormFile? formFile);
        Task<ResponseData<Product>> UpdateProductAsync(int id, Product product, IFormFile? formFile);
        Task<ResponseData<Product>> DeleteProductAsync(int id);
    }
}

