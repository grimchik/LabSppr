using Bar.Domain.Entities;
using Bar.Domain.Models;
using Bar.UI.Services.CategoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bar.UI.Services.ProductService
{
    /*public class MemoryProductService : IProductService
    {
        private  List<Product> _products;
        private  List<Category> _categories;
        private readonly IConfiguration _config;
        private readonly ICategoryService _categoryService;

        public MemoryProductService(IConfiguration config, ICategoryService categoryService)
        {
            _config = config;
            _categoryService = categoryService;
            _categories = categoryService.GetCategoryListAsync().Result.Data;
            SetupData();
        }

        private void SetupData()
        {
            _products = new List<Product>
        {
            new Product
            {
                ID = 1,
                Name = "Крафтовое пиво",
                Description = "Насыщенный вкус, крепость 5%",
                Price = 300,
                ImagePath = "Images/craft-beer.jpg",
                ImageMimeType = "image/jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("beer"))
            },
            new Product
            {
                ID = 2,
                Name = "Бурбон",
                Description = "Виски из кукурузы",
                Price = 500,
                ImagePath = "Images/bourbon.jpg",
                ImageMimeType = "image/jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("whiskey"))
            },
            new Product
            {
                ID = 3,
                Name = "Шот текилы",
                Description = "Отличный выбор для шотов",
                Price = 200,
                ImagePath = "Images/tequila-shot.jpg",
                ImageMimeType = "image/jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("shots"))
            },
            new Product
            {
                ID = 4,
                Name = "Красное вино",
                Description = "Полусладкое вино с ягодным привкусом",
                Price = 450,
                ImagePath = "Images/red-wine.jpg",
                ImageMimeType = "image/jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("wine"))
            },
            new Product
            {
                ID = 5,
                Name = "Мохито",
                Description = "Освежающий коктейль с мятой",
                Price = 350,
                ImagePath = "Images/mojito.jpg",
                ImageMimeType = "image/jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("cocktails"))
            }
        };
        }

        public Task<ResponseData<ListModel<Product>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var products = _products
                .Where(p => string.IsNullOrEmpty(categoryNormalizedName) || p.Category.NormalizedName.Equals(categoryNormalizedName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            int itemsPerPage = _config.GetValue<int>("ItemsPerPage");

            int totalItems = products.Count;

            int totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);

            var paginatedProducts = products
                .Skip((pageNo - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var listModel = new ListModel<Product>
            {
                Items = paginatedProducts,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            return Task.FromResult(ResponseData<ListModel<Product>>.Success(listModel));
        }


        public Task<ResponseData<Product>> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ID == id);
            return Task.FromResult(product != null
                ? ResponseData<Product>.Success(product)
                : ResponseData<Product>.Error("Продукт не найден", null));
        }

        public Task UpdateProductAsync(int id, Product product, IFormFile? formFile)
        {
            var existingProduct = _products.FirstOrDefault(p => p.ID == id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                if (formFile != null)
                {
                    existingProduct.ImagePath = $"Images/{formFile.FileName}";
                    existingProduct.ImageMimeType = formFile.ContentType;
                }
            }

            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ID == id);
            if (product != null)
            {
                _products.Remove(product);
            }

            return Task.CompletedTask;
        }

        public Task<ResponseData<Product>> CreateProductAsync(Product product, IFormFile? formFile)
        {
            product.ID = _products.Max(p => p.ID) + 1;
            if (formFile != null)
            {
                product.ImagePath = $"Images/{formFile.FileName}";
                product.ImageMimeType = formFile.ContentType;
            }

            _products.Add(product);
            return Task.FromResult(ResponseData<Product>.Success(product));
        }
    }*/
}
