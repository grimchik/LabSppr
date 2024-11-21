using Bar.Domain.Entities;
using Bar.Domain.Models;
using Bar.UI.Services.CategoryService;
using Bar.UI.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace Lab1_sppr.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ICategoryService categoryService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? category = null, int pageNo = 1)
        {
            _logger.LogInformation("Fetching categories...");
            var response = await _categoryService.GetCategoryListAsync();

            if (response == null || response.Data == null)
            {
                _logger.LogWarning("Categories response is null.");
                return View(new ListModel<Product>());
            }

            _logger.LogInformation($"Categories fetched: {response.Data.Count}");
            var categories = response.Data;
            var currentCategory = categories.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";
            ViewData["categories"] = categories;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentCategoryNormalizedName"] = category;

            _logger.LogInformation("Fetching product list...");
            Console.WriteLine(category);
            var products = await _productService.GetProductListAsync(category, pageNo);
            
            if (products == null || products.Data == null)
            {
                _logger.LogWarning("Products response is null.");
                return View(new ListModel<Product>()); // Возвращаем пустую модель, если нет данных
            }

            _logger.LogInformation($"Products fetched: {products.Data.Items.Count}");
            var model = products.Data;
            Console.WriteLine(model);
            return View(model);
        }
    }
}
