using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.Core.Services;

namespace HaberApp.Services.Services
{
    public class CategoryNewsUrlService : ServiceBase<CategoryNewsUrlRequestDto, CategoryNewsUrl, CategoryNewsUrlResponseDto>, ICategoryNewsUrlService
    {
    }
}
