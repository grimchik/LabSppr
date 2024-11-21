using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bar.Domain.Entities;
using Bar.UI.Services.ProductService;

namespace Lab1_sppr.Areas.Admin.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService; // Используем IProductService

        public DetailsModel(IProductService productService)
        {
            _productService = productService; // Внедрение зависимости
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _productService.GetProductByIdAsync(id.Value);
            if (response.Data == null)
            {
                Console.WriteLine("No data retrieved for the product ID: " + id); // Сообщение, если данных нет
                return NotFound();
            }
            else
            {
                Product = response.Data;
                Console.WriteLine($"Product loaded: {Product.Name}, Image: {Product.ImagePath}"); // Проверка загрузки данных
            }
            return Page();
        }

    }
}
