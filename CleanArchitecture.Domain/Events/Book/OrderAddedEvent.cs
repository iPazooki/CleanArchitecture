namespace CleanArchitecture.Domain.Events.Book;

public class BookAddedEvent(Entities.Book book) : INotification
{
    public Entities.Book Book { get; } = book;
}
