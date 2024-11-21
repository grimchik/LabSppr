using BAR.API.Data;
using Bar.Domain.Models;
using Bar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Bar.API.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly int _maxPageSize = 20;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseData<ListModel<Product>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3)
        {
            Console.WriteLine(categoryNormalizedName);
            if (pageSize > _maxPageSize)
                pageSize = _maxPageSize;

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(categoryNormalizedName))
            {
                query = query.Where(d => d.Category.NormalizedName.Equals(categoryNormalizedName));
            }

            var count = await query.CountAsync();
            var dataList = new ListModel<Product>();

            if (count == 0)
            {
                return ResponseData<ListModel<Product>>.Success(dataList);
            }

            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNo > totalPages)
                return ResponseData<ListModel<Product>>.Error("No such page");

            dataList.Items = await query
                .OrderBy(d => d.ID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;

            return ResponseData<ListModel<Product>>.Success(dataList);
        }

        public async Task<ResponseData<Product>> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(p => p.ID == id);

            return product != null
                ? ResponseData<Product>.Success(product)
                : ResponseData<Product>.Error("Product not found");
        }

        public async Task<ResponseData<Product>> UpdateProductAsync(int id, Product product, IFormFile? formFile)
        {
            if (id != product.ID)
            {
                throw new ArgumentException("Product ID mismatch");
            }
            if (formFile != null)
            {
                var imageUrl = await SaveImageAsync(product.ID, formFile);
                if (!string.IsNullOrEmpty(imageUrl.Data))
                {
                    product.ImagePath = imageUrl.Data;
                }
            }
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return ResponseData<Product>.Success(product);
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ResponseData<Product>> CreateProductAsync(Product product)
        {
            // Проверяем, существует ли категория с таким названием (или другим уникальным полем)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == product.Category.Name);

            if (existingCategory != null)
            {
                // Если категория существует, привязываем продукт к существующей категории
                product.Category = existingCategory;
            }
            else
            {
                // Если категория не существует, добавляем новую категорию
                _context.Categories.Add(product.Category);
            }

            // Добавляем продукт в базу данных
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return ResponseData<Product>.Success(product);
        }



        public async Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile)
        {
            string imageUrl = $"https://localhost:7002/Images/{formFile.FileName}";
            using var stream = new FileStream(Path.Combine("wwwroot/Images", formFile.FileName), FileMode.Create);
            await formFile.CopyToAsync(stream);
            return ResponseData<string>.Success(imageUrl);
        }

        public async Task UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Продукт не найден");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImagePath = product.ImagePath; // Обновите, если нужно

            await _context.SaveChangesAsync();
        }

    }
}
