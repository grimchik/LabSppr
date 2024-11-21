using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bar.Domain.Entities;
using Bar.UI.Services.ProductService;

namespace Lab1_sppr.Areas.Admin.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Product> Products { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string CurrentCategoryNormalizedName { get; set; }

        public async Task OnGetAsync(string categoryNormalizedName, int pageNo = 1, int pageSize = 3)
        {
            var response = await _productService.GetProductListAsync(categoryNormalizedName, pageNo, pageSize);
            if (response.Successful && response.Data != null)
            {
                Products = response.Data.Items;
                CurrentPage = response.Data.CurrentPage;
                TotalPages = response.Data.TotalPages;
                CurrentCategoryNormalizedName = categoryNormalizedName;
            }
        }
    }
}
