using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bar.Domain.Entities;
using Bar.UI.Services.ProductService;
using Bar.UI.Services.CategoryService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab1_sppr.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService; // Используем IProductService вместо DbContext
        private readonly ICategoryService _categoryService;

        [BindProperty]
        public IFormFile? UploadFile { get; set; } // Новый параметр для загрузки файла
        public EditModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
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

            var response = await _productService.GetProductByIdAsync(id.Value);
            if (response.Data == null)
            {
                return NotFound();
            }

            Product = response.Data;

            var categories = await _categoryService.GetCategoryListAsync();
            ViewData["CategoryId"] = new SelectList(categories.Data, "ID", "Name", Product.CategoryID);

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _categoryService.GetCategoryListAsync();
            var categories = response.Data ?? new List<Category>();

            // Находим выбранную категорию
            Product.Category = categories.FirstOrDefault(c => c.ID == Product.CategoryID);
            await _productService.UpdateProductAsync(Product.ID, Product, UploadFile);
            return RedirectToPage("./Index");
        }



        private async Task<bool> ProductExists(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return response.Data != null; // Проверяем, существует ли продукт
        }
    }
}
