using Bar.Domain;
using Bar.Domain.Entities;
using Bar.Domain.Models;
namespace Bar.API.Services
{
    public interface ICategoryService
    {
        Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
