using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.DTOs.CategoryNewsUrl;

namespace HaberApp.Services.Mappers
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Category, CategoryResponseDto>();

            CreateMap<CategoryNewsUrlRequestDto, CategoryNewsUrl>();
            CreateMap<CategoryNewsUrl, CategoryNewsUrlResponseDto>();

            CreateMap<ArticleRequestDto, Article>();
            CreateMap<Article, ArticleResponseDto>();
        }
    }
}
