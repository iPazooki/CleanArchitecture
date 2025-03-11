namespace CleanArchitecture.Domain.Entities;

public partial class Book
{
    private Book() { }

    public static Result<Book> Create(string title, Genre genre)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Book>.Failure(BookErrors.TitleIsRequired);
        }

        Book book = new()
        {
            Title = title,
            Genre = genre
        };

        return Result<Book>.Success(book);
    }
}
