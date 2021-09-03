using FluentValidation;

namespace Ca.Services.DTOs.Blog
{
    public class BlogCommentDtoValidator : BaseDtoValidator<BlogCommentDto>
    {
        public BlogCommentDtoValidator()
        {
            RuleFor(x => x.BlogPostId)
                .NotNull();

            RuleFor(x => x.CommentText)
                .NotNull()
                .MinimumLength(10);

            RuleFor(x => x.CreatedOn)
                .NotNull();
        }
    }
}