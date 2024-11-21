using BAR.API.Data; 
using Microsoft.EntityFrameworkCore;
using Bar.Domain;
using Bar.API.Services;
using Bar.UI.Services;
using Bar.UI;
using Bar.UI.Services.FileService;
using Bar.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
// ����������� � ������������ Swagger/OpenAPI ����� ����� �� ������ https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// �������� ������ ����������� �� ������������
string connectionString = builder.Configuration.GetConnectionString("Default"); 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
var authServer = builder.Configuration
.GetSection("AuthServer")
.Get<AuthServerData>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    // ����� ���������� ������������ OpenID
    o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
    // Authority ������� ��������������
    o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
    // Audience ��� ������ JWT
    o.Audience = "account";
    // ��������� HTTPS ��� ������������� ��������� ������ Keycloak
    // � ������� ������� ������ ���� true
    o.RequireHttpsMetadata = false;
});
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});
var app = builder.Build();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");// ����������� ���� ������ �����������

await DbInitializer.SeedData(app);
// ����������� �������� ��������� HTTP-��������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
