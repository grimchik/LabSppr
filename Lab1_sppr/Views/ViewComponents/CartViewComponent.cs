using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lab1_sppr.ViewComponents
{
    public class Cart : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartInfo = new CartInfo
            {
                TotalAmount = 0.0m,
                ItemCount = 0       
            };

            return View(cartInfo);
        }
    }

    public class CartInfo
    {
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
