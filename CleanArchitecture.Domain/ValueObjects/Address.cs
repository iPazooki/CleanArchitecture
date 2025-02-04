namespace CleanArchitecture.Domain.ValueObjects;

/// <summary>
/// Represents an address with a city, street, and postal code.
/// </summary>
/// <param name="City">The city of the address.</param>
/// <param name="Street">The street of the address.</param>
/// <param name="PostalCode">The postal code of the address.</param>
public sealed record Address(string City, string Street, string PostalCode);