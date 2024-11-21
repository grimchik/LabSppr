using Microsoft.AspNetCore.Mvc;
using Bar.API.Services;
using Bar.Domain.Entities;
using Bar.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Bar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger; // Объявите поле для логгера

        public ProductsController(IProductService productService, ILogger<ProductsController> logger) // Измените конструктор
        {
            _productService = productService;
            _logger = logger; // Инициализируйте логгер
        }

        [HttpGet("{category?}")]
        //[Authorize(Policy = "admin")]
        public async Task<ActionResult<ResponseData<ListModel<Product>>>> GetProducts(string? category, int pageNo = 1, [FromQuery] int pageSize = 3)
        {
            // Логирование переданных параметров
            var parameters = new
            {
                Category = category,
                PageNo = pageNo,
                PageSize = pageSize
            };

            _logger.LogInformation($"Received parameters: {JsonSerializer.Serialize(parameters)}");

            var result = await _productService.GetProductListAsync(category, pageNo, pageSize);
            if (result == null || !result.Successful)
            {
                return NotFound(result?.ErrorMessage);
            }
            return Ok(result);
        }

        // GET: api/Products/5
        [HttpGet("{id:int}")]
        //[Authorize(Policy = "admin")]
        public async Task<ActionResult<ResponseData<Product>>> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Successful)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // PUT: api/Products/5
        [HttpPut("{id:int}")]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest(ResponseData<string>.Error("Product ID mismatch"));
            }

            try
            {
                await _productService.UpdateProductAsync(id, product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(id))
                {
                    return NotFound(ResponseData<string>.Error("Product not found"));
                }
                else
                {
                    throw; // повторно выбрасываем исключение, если оно не связано с отсутствием продукта
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        [Authorize(Policy = "admin")]
        public async Task<ActionResult<ResponseData<Product>>> PostProduct([FromBody] Product product)
        {
            // Передаем product и formFile в сервис для обработки
            var result = await _productService.CreateProductAsync(product);

            // Если создание продукта не удалось, возвращаем ошибку
            if (!result.Successful)
            {
                return BadRequest(result); // Обработка ошибки создания продукта
            }

            // Возвращаем успешный результат с кодом 201 (Created)
            return CreatedAtAction(nameof(GetProduct), new { id = result.Data?.ID }, result);
        }


        // DELETE: api/Products/5
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        private async Task<bool> ProductExists(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return result.Data != null;
        }
    }
}
