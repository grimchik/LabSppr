using Bar.Domain.Entities;
using Bar.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bar.UI.Services.CategoryService;
using Bar.UI.Services.ProductService;
using Bar.Domain.Entities;
using Bar.Domain.Models;
using Lab1_sppr.Controllers;
namespace Bar
{
    public class ProductControllerTests
    {
        private readonly ICategoryService _mockCategoryService;
        private readonly IProductService _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            // Mock the services
            _mockCategoryService = Substitute.For<ICategoryService>();
            _mockProductService = Substitute.For<IProductService>();

            // Initialize the controller with mocks
            _controller = new ProductController(_mockProductService, _mockCategoryService, Substitute.For<ILogger<ProductController>>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() // Mock HttpContext
                }
            };
        }

        [Fact]
        public async Task Index_Returns404_WhenCategoryListNotSuccessful()
        {
            // Arrange
            var mockCategoryService = Substitute.For<ICategoryService>();
            var mockProductService = Substitute.For<IProductService>();
            var mockLogger = Substitute.For<ILogger<ProductController>>();

            // Настраиваем мок так, чтобы он возвращал неуспешный ответ
            mockCategoryService
                .GetCategoryListAsync()
                .Returns(ResponseData<List<Category>>.Error("Failed to load categories"));

            // Создаем контроллер с моками
            var controller = new ProductController(mockProductService, mockCategoryService, mockLogger)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() // Имитация HttpContext
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Failed to load categories", notFoundResult.Value);
        }



        [Fact]
        public async Task Index_Returns404_WhenProductListNotSuccessful()
        {
            // Arrange
            _mockCategoryService.GetCategoryListAsync().Returns(ResponseData<List<Category>>.Success(new List<Category>()));
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(ResponseData<ListModel<Product>>.Error("Product fetch failed"));

            // Act
            var result = await _controller.Index();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product fetch failed", notFoundResult.Value);
        }

        [Fact]
        public async Task Index_PassesCategoriesToViewData()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { ID = 1, Name = "Drinks", NormalizedName = "drinks" },
                new Category { ID = 2, Name = "Snacks", NormalizedName = "snacks" }
            };
            _mockCategoryService.GetCategoryListAsync().Returns(ResponseData<List<Category>>.Success(categories));
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(ResponseData<ListModel<Product>>.Success(new ListModel<Product>()));

            // Act
            await _controller.Index("drinks");

            // Assert
            Assert.Equal(categories, _controller.ViewData["categories"]);
        }

        [Fact]
        public async Task Index_SetsCorrectCurrentCategoryInViewData()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { ID = 1, Name = "Drinks", NormalizedName = "drinks" },
                new Category { ID = 2, Name = "Snacks", NormalizedName = "snacks" }
            };
            _mockCategoryService.GetCategoryListAsync().Returns(ResponseData<List<Category>>.Success(categories));
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(ResponseData<ListModel<Product>>.Success(new ListModel<Product>()));

            // Act
            await _controller.Index("drinks");

            // Assert
            Assert.Equal("Drinks", _controller.ViewData["currentCategory"]);
        }

        [Fact]
        public async Task Index_SetsDefaultCategory_WhenNoCategoryProvided()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { ID = 1, Name = "Drinks", NormalizedName = "drinks" },
                new Category { ID = 2, Name = "Snacks", NormalizedName = "snacks" }
            };
            _mockCategoryService.GetCategoryListAsync().Returns(ResponseData<List<Category>>.Success(categories));
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(ResponseData<ListModel<Product>>.Success(new ListModel<Product>()));

            // Act
            await _controller.Index();

            // Assert
            Assert.Equal("Все", _controller.ViewData["currentCategory"]);
        }

        [Fact]
        public async Task Index_PassesProductListToView()
        {
            // Arrange
            var products = new ListModel<Product>
            {
                Items = new List<Product>
                {
                    new Product { ID = 1, Name = "Coke" },
                    new Product { ID = 2, Name = "Pepsi" }
                }
            };
            _mockCategoryService.GetCategoryListAsync().Returns(ResponseData<List<Category>>.Success(new List<Category>()));
            _mockProductService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(ResponseData<ListModel<Product>>.Success(products));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(products, viewResult.Model);
        }
    }
}
