using AutoMapper;
using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.AppUser;
using HaberApp.Core.DTOs.Article;
using HaberApp.Core.DTOs.Category;
using HaberApp.Core.DTOs.CategoryNewsUrl;
using HaberApp.Core.DTOs.Message;
using HaberApp.Core.DTOs.Others;

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

            CreateMap<MessageRequestDto, Message>();
            CreateMap<Message, MessageResponseDto>();

            //CreateMap<AppRole, RoleResponseDto>()
            //     .ForCtorParam(nameof(RoleResponseDto.RoleId), options => options.MapFrom(user => user.Id))
            //    .ForMember(a => a.Id, opt => opt.Ignore());
            CreateMap<AppRole, RoleResponseDto>()
                .ConstructUsing(src => new RoleResponseDto(
                    src.Id,
                    src.Name ?? string.Empty,
                    null
                ))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<AppUserRequestDto, AppUser>();

            CreateMap<AppUser, AppUserResponseDto>()
                .ForCtorParam(nameof(AppUserResponseDto.UserId), options => options.MapFrom(user => user.Id))
                .ForMember(response => response.Id, options => options.Ignore());

        }
    }
}
