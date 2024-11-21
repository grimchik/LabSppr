using Bar.Domain.Entities;
using Bar.Domain.Models;

namespace Bar.UI.Services.CategoryService
{
    /*public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
            {
                new Category {ID = 1, Name = "Пиво", NormalizedName = "beer"},
                new Category {ID = 2, Name = "Виски", NormalizedName = "whiskey"},
                new Category {ID = 3, Name = "Вино", NormalizedName = "wine"},
                new Category {ID = 4, Name = "Шоты", NormalizedName = "shots"},
                new Category {ID = 5, Name = "Коктейли", NormalizedName = "cocktails"}
            };
            var result = ResponseData<List<Category>>.Success(categories);
            return Task.FromResult(result);
        }
    }*/
}
