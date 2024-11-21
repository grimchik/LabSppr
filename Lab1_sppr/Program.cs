using Bar.API.Services;
using Bar.UI;
using Bar.UI.Extensions;
using Bar.UI.Services.FileService;
using Bar.UI.Services.CategoryService;
using Bar.UI.Services.ProductService;
using BAR.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ICategoryService = Bar.API.Services.ICategoryService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Configuration;
using Bar.UI.HelperClasses;
using Bar.UI.Services.TokenAccessor;
using Bar.UI.Services.AuthService;
using System.Net.Http.Headers;
using Bar.UI.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.RegisterCustomServices();
builder.Services.AddRazorPages();
var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
string connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddScoped<IAuthService, KeycloakAuthService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddHttpClient<IFileService, Bar.UI.Services.ApiFileService>(opt =>
{
    opt.BaseAddress = new Uri($"{uriData.ApiUri}Files");
});
builder.Services.AddHttpClient<Bar.UI.Services.ProductService.IProductService, ApiProductService>(opt =>
{
    var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
    opt.BaseAddress = new Uri(uriData.ApiUri);
});
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddHttpClient<Bar.UI.Services.CategoryService.ICategoryService, ApiCategoryService>(opt =>
{
    var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
    opt.BaseAddress = new Uri(uriData.ApiUri);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenAccessor, KeycloakTokenAccessor>();
var keycloakData = builder.Configuration.GetSection("Keycloak").Get<KeycloakData>();
builder.Services
.AddAuthentication(options =>
{
    options.DefaultScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
    OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddJwtBearer()
.AddOpenIdConnect(options =>
{
    options.Authority =
    $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
    options.ClientId = keycloakData.ClientId;
    options.ClientSecret = keycloakData.ClientSecret;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Add("openid"); // Customize scopes as needed
    options.SaveTokens = true;
    options.RequireHttpsMetadata = false; 
options.MetadataAddress =$"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
});
var app = builder.Build();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();