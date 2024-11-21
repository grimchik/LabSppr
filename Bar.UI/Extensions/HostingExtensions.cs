using Bar.UI.Services.CategoryService;
using Bar.UI.Services.ProductService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bar.UI.HelperClasses;

namespace Bar.UI.Extensions
{
    public static class HostingExtensions
    {
        public static void RegisterCustomServices(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));
            //builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();
            //builder.Services.AddScoped<IProductService, MemoryProductService>();
        }
    }
}
