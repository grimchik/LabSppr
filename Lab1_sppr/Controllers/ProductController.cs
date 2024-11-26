using Bar.Domain.Entities;
using Bar.Domain.Models;
using Bar.UI.Services.CategoryService;
using Bar.UI.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace Lab1_sppr.Controllers
{
    [Route("Catalog")]
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

        [Route("")]
        [Route("{category?}")]
        public async Task<IActionResult> Index(string? category = null, int pageNo = 1)
        {
            _logger.LogInformation("Fetching categories...");
            var response = await _categoryService.GetCategoryListAsync();

            // Проверка успешности ответа
            if (response == null || !response.Successful || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch categories: {ErrorMessage}", response?.ErrorMessage);
                return NotFound(response?.ErrorMessage ?? "Categories not found."); // Возвращаем 404
            }

            _logger.LogInformation($"Categories fetched: {response.Data.Count}");
            var categories = response.Data;

            var currentCategory = categories.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";
            ViewData["categories"] = categories;
            ViewData["currentCategory"] = currentCategory;
            ViewData["currentCategoryNormalizedName"] = category;

            _logger.LogInformation("Fetching product list...");
            var products = await _productService.GetProductListAsync(category, pageNo);

            if (products == null || !products.Successful || products.Data == null)
            {
                _logger.LogWarning("Failed to fetch products: {ErrorMessage}", products?.ErrorMessage);

                var emptyModel = new ListModel<Product>();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    _logger.LogInformation("Returning partial view for empty product list.");
                    return PartialView("_ListPartial", emptyModel);
                }

                return NotFound(products?.ErrorMessage ?? "Products not found."); // Возвращаем 404
            }

            _logger.LogInformation($"Products fetched: {products.Data.Items.Count}");
            var model = products.Data;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                _logger.LogInformation("Returning partial view for products.");
                return PartialView("_ListPartial", model);
            }

            _logger.LogInformation("Returning full view for products.");
            return View(model);
        }
    }
}
