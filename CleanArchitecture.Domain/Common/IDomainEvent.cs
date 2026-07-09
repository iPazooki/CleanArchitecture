using System.Diagnostics.CodeAnalysis;

namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Marks a record of something that happened in the domain.
/// </summary>
/// <remarks>
/// Deliberately free of any messaging-library type. The Application layer adapts these to
/// its mediator by wrapping them in a notification, which is what keeps this project
/// dependency-free.
/// </remarks>
[SuppressMessage(
    "Design",
    "CA1040:Avoid empty interfaces",
    Justification = "Marker interface: it constrains DomainEventNotification<T> and lets the " +
                    "persistence interceptor collect events without the Domain knowing about the mediator.")]
public interface IDomainEvent;
