using FluentValidation;

namespace Ca.Services.DTOs
{
    public abstract class BaseDtoValidator<TEntity> : AbstractValidator<TEntity> where TEntity : BaseDto
    {
        public BaseDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull();
        }
    }
}