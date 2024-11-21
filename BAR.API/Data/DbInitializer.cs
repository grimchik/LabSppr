using BAR.API.Data; // Проверьте правильность пути
using Bar.Domain; // Проверьте правильность пути
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Bar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BAR.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            if (context.Categories.Any() || context.Products.Any())
            {
                return; 
            }

            var BaseUrl = app.Configuration["BaseUrl"];

            var categories = new List<Category>
            {
                new Category { Name = "Beer", NormalizedName = "beer" },
                new Category { Name = "Whiskey", NormalizedName = "whiskey" },
                new Category { Name = "Wine", NormalizedName = "wine" },
                new Category { Name = "Shots", NormalizedName = "shots" },
                new Category { Name = "Cocktails", NormalizedName = "cocktails" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync(); 

            var products = new List<Product>
            {
                new Product { Name = "IPA Beer", Description = "A hoppy beer.", Price = 5.00m, ImagePath = $"{BaseUrl}/Images/craft-beer.jpg", ImageMimeType = "image/jpeg", CategoryID = categories.First(c => c.NormalizedName == "beer").ID },
                new Product { Name = "Single Malt Whiskey", Description = "Smooth and rich.", Price = 15.00m, ImagePath = $"{BaseUrl}/Images/bourbon.jpg", ImageMimeType = "image/jpeg", CategoryID = categories.First(c => c.NormalizedName == "whiskey").ID },
                new Product { Name = "Red Wine", Description = "Full-bodied red wine.", Price = 10.00m, ImagePath = $"{BaseUrl}/Images/red-wine.jpg", ImageMimeType = "image/jpeg", CategoryID = categories.First(c => c.NormalizedName == "wine").ID },
                new Product { Name = "Tequila Shot", Description = "Classic tequila shot.", Price = 4.00m, ImagePath = $"{BaseUrl}/Images/tequila-shot.jpg", ImageMimeType = "image/jpeg", CategoryID = categories.First(c => c.NormalizedName == "shots").ID },
                new Product { Name = "Mojito", Description = "Refreshing cocktail.", Price = 8.00m, ImagePath = $"{BaseUrl}/Images/mojito.jpg", ImageMimeType = "image/jpeg", CategoryID = categories.First(c => c.NormalizedName == "cocktails").ID }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync(); 
        }
    }
}
