using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Persistence.Data;
using CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.UnitTests;

public class AuditableEntityInterceptorTests
{
    [Fact]
    public void SavingChanges_ShouldPopulateUpdatedBy_WithCurrentUserName()
    {
        // Arrange
        const string expectedUser = "test-username";
        var timeProvider = TimeProvider.System;
        var currentUser = new TestCurrentUser(expectedUser);
        var interceptor = new AuditableEntityInterceptor(timeProvider, currentUser);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=dummy;")
            .AddInterceptors(interceptor)
            .Options;

        using var context = new ApplicationDbContext(options, NullLogger<ApplicationDbContext>.Instance);

        var bookResult = Book.Create("Test Title", Genre.Fiction);
        var book = bookResult.Value!;
        context.Books.Add(book);

        // Act
        try
        {
            context.SaveChanges();
        }
        catch
        {
            // Ignore database connection error since interceptor runs before the database call
        }

        // Assert
        Assert.Equal(expectedUser, book.UpdatedBy);
    }

    [Fact]
    public void SavingChanges_ShouldWork_WhenCurrentUserNameIsNull()
    {
        // Arrange
        var timeProvider = TimeProvider.System;
        var currentUser = new TestCurrentUser(null);
        var interceptor = new AuditableEntityInterceptor(timeProvider, currentUser);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=dummy;")
            .AddInterceptors(interceptor)
            .Options;

        using var context = new ApplicationDbContext(options, NullLogger<ApplicationDbContext>.Instance);

        var bookResult = Book.Create("Test Title", Genre.Fiction);
        var book = bookResult.Value!;
        context.Books.Add(book);

        // Act
        try
        {
            context.SaveChanges();
        }
        catch
        {
            // Ignore database connection error
        }

        // Assert
        Assert.Null(book.UpdatedBy);
    }

    private sealed class TestCurrentUser(string? username) : ICurrentUser
    {
        public string? UserName => username;
    }
}
