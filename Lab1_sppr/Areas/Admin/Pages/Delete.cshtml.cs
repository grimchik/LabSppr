using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bar.Domain.Entities;
using Bar.UI.Services.ProductService;
using Bar.UI.Services.CategoryService;

namespace Lab1_sppr.Areas.Admin.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly IProductService _productService; // Используйте интерфейс
        private readonly ICategoryService _categoryService;

        public DeleteModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService; // Внедрение зависимости
            _categoryService = categoryService;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _productService.GetProductByIdAsync(id.Value); // Получаем деталь по ID

            if (response.Data == null)
            {
                return NotFound();
            }
            else
            {
                Product = response.Data; // Присваиваем значение Detail
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _productService.GetProductByIdAsync(id.Value);
            if (response.Data != null)
            {
                await _productService.DeleteProductAsync(id.Value); // Метод для удаления
            }

            return RedirectToPage("./Index");
        }
    }
}
