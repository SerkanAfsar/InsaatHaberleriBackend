using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.CategoryNewsUrl;

namespace HaberApp.Core.Services
{
    public interface ICategoryNewsUrlService : IServiceBase<CategoryNewsUrlRequestDto, CategoryNewsUrl, CategoryNewsUrlResponseDto>
    {
    }
}
