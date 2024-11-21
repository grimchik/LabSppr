using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bar.Domain.Entities;
using Bar.UI.Services.ProductService;
using Bar.UI.Services.CategoryService;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab1_sppr.Areas.Admin.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;


        public CreateModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task OnGetAsync()
        {

            var response = await _categoryService.GetCategoryListAsync();


            if (response?.Data != null)
            {
                ViewData["CategoryId"] = new SelectList(response.Data, "ID", "Name");
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(new List<Category>(), "ID", "Name");
            }
        }

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Загрузка категорий из сервиса (кеширование на уровне формы)
            var response = await _categoryService.GetCategoryListAsync();
            var categories = response.Data ?? new List<Category>();

            // Находим выбранную категорию
            Product.Category = categories.FirstOrDefault(c => c.ID == Product.CategoryID);

            // Создаем продукт с учетом загруженного файла
            await _productService.CreateProductAsync(Product, UploadFile);

            return RedirectToPage("./Index");
        }

    }
}
