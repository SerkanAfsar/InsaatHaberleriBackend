using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.Services;

namespace HaberApp.Services.Services
{
    public class CategoryService : ServiceBase<CategoryRequestDto, Category, CategoryResponseDto>, ICategoryService
    {
    }
}
