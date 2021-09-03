using FluentValidation;

namespace Ca.Services.DTOs.Blog
{
    public class BlogPostDtoValidator : BaseDtoValidator<BlogPostDto>
    {
        public BlogPostDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(3);

            RuleFor(x => x.Body)
                .NotNull()
                .MinimumLength(100);

            RuleFor(x => x.CreatedOn)
                .NotNull();
        }
    }
}