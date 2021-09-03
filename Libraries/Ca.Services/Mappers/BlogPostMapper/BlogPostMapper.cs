using AutoMapper;
using Ca.Core.Domain.Blog;
using Ca.Services.DTOs.Blog;

namespace Ca.Services.Mappers
{
    public class BlogPostMapper : Profile
    {
        public BlogPostMapper()
        {
            CreateMap<BlogPostDto, BlogPost>()
                .ForMember(x => x.Comments, c => c.Ignore());

            CreateMap<BlogPost, BlogPostDto>();

            CreateMap<BlogPost, BlogPostWithCommentDto>();

            CreateMap<BlogCommentDto, BlogComment>()
                .ForMember(x => x.BlogPost, c => c.Ignore());

            CreateMap<BlogComment, BlogCommentDto>();
        }
    }
}