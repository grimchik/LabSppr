using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bar.Domain.Entities;
using Bar.Domain.Models;
using Bar.API.Services;
using Xunit;
using BAR.API.Data;

namespace Bar.API.Tests
{
    public class ProductServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            var context = new AppDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task ServiceReturnsFirstPageOfThreeItems()
        {
            using var context = CreateContext();

            // Arrange: Add test data
            for (int i = 1; i <= 5; i++)
            {
                context.Products.Add(new Product { Name = $"Product {i}" });
            }
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetProductListAsync(null);

            // Assert
            Assert.True(result.Successful);
            Assert.Equal(1, result.Data.CurrentPage);
            Assert.Equal(3, result.Data.Items.Count);
            Assert.Equal(2, result.Data.TotalPages);
            Assert.Equal("Product 1", result.Data.Items[0].Name);
        }

        [Fact]
        public async Task ServiceReturnsCorrectPage()
        {
            using var context = CreateContext();

            // Arrange: Add test data
            for (int i = 1; i <= 5; i++)
            {
                context.Products.Add(new Product { Name = $"Product {i}" });
            }
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetProductListAsync(null, pageNo: 2, pageSize: 3);

            // Assert
            Assert.True(result.Successful);
            Assert.Equal(2, result.Data.CurrentPage);
            Assert.Equal(2, result.Data.Items.Count); // Ожидается 2 продукта на второй странице
            Assert.Equal("Product 4", result.Data.Items[0].Name);
            Assert.Equal("Product 5", result.Data.Items[1].Name);
        }


        [Fact]
        public async Task ServiceFiltersByCategory()
        {
            using var context = CreateContext();

            // Arrange: Add test data
            var category = new Category { Name = "Test Category", NormalizedName = "test-category" };
            context.Categories.Add(category);
            context.Products.Add(new Product { Name = "Filtered Product", Category = category });
            context.Products.Add(new Product { Name = "Other Product" });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetProductListAsync("test-category");

            // Assert
            Assert.True(result.Successful);
            Assert.Single(result.Data.Items);
            Assert.Equal("Filtered Product", result.Data.Items[0].Name);
        }

        [Fact]
        public async Task ServiceDoesNotAllowPageSizeExceedingMax()
        {
            using var context = CreateContext();

            // Arrange: Add test data
            for (int i = 1; i <= 25; i++)
            {
                context.Products.Add(new Product { Name = $"Product {i}" });
            }
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetProductListAsync(null, pageSize: 50);

            // Assert
            Assert.True(result.Successful);
            Assert.Equal(1, result.Data.CurrentPage);
            Assert.Equal(20, result.Data.Items.Count); // Max page size is 20
        }

        [Fact]
        public async Task ServiceReturnsErrorForInvalidPageNumber()
        {
            using var context = CreateContext();

            // Arrange: Add test data
            for (int i = 1; i <= 3; i++)
            {
                context.Products.Add(new Product { Name = $"Product {i}" });
            }
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetProductListAsync(null, pageNo: 5);

            // Assert
            Assert.False(result.Successful);
            Assert.Equal("No such page", result.ErrorMessage);
        }
    }
}
