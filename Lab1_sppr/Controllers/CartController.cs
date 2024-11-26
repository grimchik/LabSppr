using Bar.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bar.UI.Services.ProductService;
using Bar.UI.Extensions;
namespace Lab1_sppr.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly Cart _cart;

        public CartController(IProductService productService, Cart cart)
        {
            _productService = productService;
            _cart = cart; // Внедрённая корзина
        }

        [HttpPost("add/{id:int}")]
        public async Task<IActionResult> Add(int id, string returnUrl)
        {
            // Получаем данные о продукте
            var productResponse = await _productService.GetProductByIdAsync(id);
            if (productResponse.Successful && productResponse.Data != null)
            {
                _cart.AddToCart(productResponse.Data); // Добавляем в корзину
            }

            return Redirect(returnUrl);
        }

        [HttpPost("remove/{id:int}")]
        public IActionResult Remove(int id, string returnUrl)
        {
            _cart.RemoveItems(id); // Удаляем из корзины
            return Redirect(returnUrl);
        }

        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            return View(_cart); // Передаём корзину в представление
        }
    }
}
