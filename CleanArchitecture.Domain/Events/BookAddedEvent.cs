namespace CleanArchitecture.Domain.Events;

/// <summary>
/// Domain event raised when a new book is added to the catalog.
/// Carries the identifier of the created book rather than the aggregate itself,
/// keeping the event an immutable, self-contained record of what happened.
/// </summary>
/// <param name="BookId">The identifier of the book that was added.</param>
public sealed record BookAddedEvent(Guid BookId) : IDomainEvent;
