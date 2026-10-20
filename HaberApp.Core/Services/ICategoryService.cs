using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Category;

namespace HaberApp.Core.Services
{
    public interface ICategoryService : IServiceBase<CategoryRequestDto, Category, CategoryResponseDto>
    {
    }
}
