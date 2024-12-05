using Bar.Domain.Entities;
using Bar.Domain.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text;

namespace Bar.BlazorWasm.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly string _pageSize;
        private readonly IAccessTokenProvider _tokenProvider;

        public event Action DataLoaded;

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Category SelectedCategory { get; set; }

        public DataService(HttpClient httpClient, IConfiguration configuration, IAccessTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiBaseUrl"];
            _pageSize = configuration["PageSize"] ?? "3";
            _tokenProvider = tokenProvider;
        }

        public async Task GetProductListAsync(int pageNo = 1)
        {
            var route = new StringBuilder("Products/");

            if (SelectedCategory != null)
            {
                route.Append($"{SelectedCategory.NormalizedName}/");
            }

            var baseRoute = route.ToString(); // Сохраняем базовый маршрут без строки запроса.

            var queryData = new List<KeyValuePair<string, string>>();

            if (pageNo > 1)
            {
                queryData.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
            }

            if (!_pageSize.Equals("3"))
            {
                queryData.Add(KeyValuePair.Create("pageSize", _pageSize));
            }

            string fullUrl = _apiBaseUrl + baseRoute; // Базовый путь без параметров.

            if (queryData.Count > 0)
            {
                // Добавляем параметры запроса, если они есть.
                fullUrl = QueryHelpers.AddQueryString(fullUrl, queryData);
            }

            try
            {
                var tokenRequest = await _tokenProvider.RequestAccessToken();
                if (tokenRequest.TryGetToken(out var token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
                }

                var response = await _httpClient.GetFromJsonAsync<ResponseData<ListModel<Product>>>(fullUrl);

                if (response != null && response.Successful)
                {
                    Products = response.Data.Items;
                    TotalPages = response.Data.TotalPages;
                    CurrentPage = pageNo;

                    DataLoaded?.Invoke(); // Генерируем событие о том, что данные загружены.
                    Success = true;
                }
                else
                {
                    Success = false;
                    ErrorMessage = response?.ErrorMessage ?? "Ошибка загрузки данных.";
                }
            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
        }


        public async Task GetCategoryListAsync()
        {

            try
            {
                var tokenRequest = await _tokenProvider.RequestAccessToken();
                if (tokenRequest.TryGetToken(out var token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
                }

                var response = await _httpClient.GetFromJsonAsync<ResponseData<List<Category>>>(_apiBaseUrl + "Categories");

                if (response != null && response.Successful && response.Data != null)
                {
                    Categories = response.Data;
                    // Выводим категории для отладки
                    Console.WriteLine("Categories loaded:");
                    foreach (var category in Categories)
                    {
                        Console.WriteLine($"Category: {category.Name}, NormalizedName: {category.NormalizedName}");
                    }
                }
                else
                {
                    ErrorMessage = "Ошибка при загрузке категорий";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка при загрузке категорий: {ex.Message}";
            }
        }

    }
}
