namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Validator for the <see cref="GetBooksQuery"/> paging parameters.
/// </summary>
internal sealed class GetBooksQueryValidator : AbstractValidator<GetBooksQuery>
{
    /// <summary>The maximum number of items that may be requested in a single page.</summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBooksQueryValidator"/> class.
    /// </summary>
    public GetBooksQueryValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");
    }
}
