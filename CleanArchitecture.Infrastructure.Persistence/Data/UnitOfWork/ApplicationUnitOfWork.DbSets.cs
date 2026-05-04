namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

public sealed partial class ApplicationUnitOfWork
{
    public IBookRepository Books => bookRepository;
}
