namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents an auditable aggregate root entity.
/// </summary>
/// <remarks>
/// This abstract class extends the <see cref="EntityAuditable"/> class and provides
/// a base for all aggregate root entities that require auditing.
/// </remarks>
public abstract class AggregateRootAuditable : EntityAuditable;
