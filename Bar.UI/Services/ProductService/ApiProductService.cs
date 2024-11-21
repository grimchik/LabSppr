using Bar.Domain.Entities;
using Bar.Domain.Models;
using Bar.UI.Services.FileService;
using Bar.UI.Services.TokenAccessor;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Bar.UI.Services.ProductService
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _defaultPageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiProductService> _logger;
        private readonly IFileService _fileService;
        private readonly ITokenAccessor _tokenAccessor;

        public ApiProductService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiProductService> logger, IFileService fileService, ITokenAccessor tokenAccessor)
        {

            _httpClient = httpClient;
            _defaultPageSize = configuration.GetValue<string>("ItemsPerPage");
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _fileService = fileService;
            _tokenAccessor = tokenAccessor;
        }


        public async Task<ResponseData<ListModel<Product>>> GetProductListAsync(string categoryNormalizedName, int pageNo, int pageSize)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            Console.WriteLine(pageSize+categoryNormalizedName + pageNo);
            var url = string.IsNullOrEmpty(categoryNormalizedName) 
              ? $"Products?pageNo={pageNo}&pageSize={pageSize}" 
              : $"Products/{categoryNormalizedName}?pageNo={pageNo}&pageSize={pageSize}";

    var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ResponseData<ListModel<Product>>>(jsonResponse, _serializerOptions);
                    if (apiResponse?.Data != null)
                    {
                        Console.WriteLine(apiResponse.Data);
                        return ResponseData<ListModel<Product>>.Success(apiResponse.Data);
                    }
                    return ResponseData<ListModel<Product>>.Error("Нет данных продуктов.");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации: {ex.Message}");
                    return ResponseData<ListModel<Product>>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"Ошибка получения данных от сервера: {response.StatusCode}");
            return ResponseData<ListModel<Product>>.Error($"Ошибка: {response.StatusCode}");
        }


        public async Task<ResponseData<Product>> GetProductByIdAsync(int id)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            var response = await _httpClient.GetAsync($"products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseData<Product>>(_serializerOptions);
            }

            _logger.LogError($"-----> Product not found. Error: {response.StatusCode}");
            return ResponseData<Product>.Error($"Product not found. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Product>> CreateProductAsync(Product product, IFormFile? formFile)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            // Установим стандартные значения для изображения
            product.ImagePath = "Images/noimage.jpg";

            // Если был передан файл, сохраняем его
            if (formFile != null)
            {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    product.ImagePath = imageUrl;
                }
            }
            
            // Ссылка на API для создания продукта
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Products");

            // Отправляем запрос на создание продукта
            var response = await _httpClient.PostAsJsonAsync(uri, product, _serializerOptions);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ResponseData<Product>>(_serializerOptions);
                return data; // Возвращаем успешный результат
            }

            // Логируем ошибку и возвращаем ошибку
            _logger.LogError($"-----> object not created. Error: {response.StatusCode}");
            return ResponseData<Product>.Error($"Объект не добавлен. Error: {response.StatusCode}");
        }


        public async Task<ResponseData<Product>> UpdateProductAsync(int id, Product product, IFormFile? formFile)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            // Если файл есть, сохраняем его
            if (formFile != null)
            {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                product.ImagePath = imageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    product.ImagePath = imageUrl;
                }
            }

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + $"Products/{id}");
            var response = await _httpClient.PutAsJsonAsync(uri, product, _serializerOptions);

            // Если ответ успешный и нет содержимого (204 No Content)
            if (response.IsSuccessStatusCode)
            {
                // Если код ответа 204 (No Content), просто возвращаем успех без десериализации
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return ResponseData<Product>.Success(null); // Возвращаем успешный ответ с null
                }

                // Если есть содержимое, десериализуем его
                var updatedProduct = await response.Content.ReadFromJsonAsync<ResponseData<Product>>(_serializerOptions);
                return updatedProduct ?? ResponseData<Product>.Error("Ошибка при десериализации ответа");
            }
            else
            {
                // Логирование ошибки и возвращение ошибки
                _logger.LogError($"Ошибка при обновлении продукта: {response.StatusCode}");
                return ResponseData<Product>.Error($"Ошибка: {response.StatusCode}");
            }
        }



        public async Task<ResponseData<Product>> DeleteProductAsync(int id)
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            var response = await _httpClient.DeleteAsync($"products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return ResponseData<Product>.Success(null);
            }

            _logger.LogError($"-----> Product not deleted. Error: {response.StatusCode}");
            return ResponseData<Product>.Error($"Product not deleted. Error: {response.StatusCode}");
        }
    }
}
