using Bar.Domain.Entities;
using Bar.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bar.UI.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {
            private readonly HttpClient _httpClient;
            private readonly string _pageSize;
            private readonly JsonSerializerOptions _serializerOptions;
            private readonly ILogger<ApiCategoryService> _logger;

            public ApiCategoryService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiCategoryService> logger)
            {
                _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
                _pageSize = configuration.GetSection("ItemsPerPage").Value ?? "10";  // Значение по умолчанию
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                _logger = logger;
            }

            //Метод для получения списка всех категорий
            public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
            {
                if (_httpClient.BaseAddress == null)
                {
                    _logger.LogError("BaseAddress is null");
                    return ResponseData<List<Category>>.Error("BaseAddress is not set.");
                }

                var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}Categories";

                // Отправляем запрос на получение списка категорий
                var response = await _httpClient.GetAsync(new Uri(urlString));

                if (response == null)
                {
                    _logger.LogError("Received null response from API.");
                    return ResponseData<List<Category>>.Error("Received null response from API.");
                }

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Ошибка десериализации: {ex.Message}");
                        return ResponseData<List<Category>>.Error($"Ошибка: {ex.Message}");
                    }
                }

                _logger.LogError($"Ошибка при получении данных: {response.StatusCode}");
                return ResponseData<List<Category>>.Error($"Данные не получены. Статус: {response.StatusCode}");
            }
        }
    }
