using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace Domain.UnitTests;

public class GenreTests
{
    [Fact]
    public void FromCode_ValidCode_ReturnsGenre()
    {
        string code = "F";
        Genre genre = Genre.FromCode(code);
        Assert.Equal(code, genre.Code);
    }

    [Fact]
    public void FromCode_ValidCodeWithCode_ReturnsGenre()
    {
        Genre genre = Genre.FromCode("F");
        Assert.Equal(Genre.Fiction.Code, genre.Code);
    }

    [Fact]
    public void ToString_ValidCode_ReturnsGenre()
    {
        Genre genre = Genre.Fiction;

        Assert.Equal(genre.Code, genre.ToString());
    }

    [Fact]
    public void ImplicitConversion_ValidCode_ReturnsGenre()
    {
        string genre = Genre.Fiction;

        Assert.Equal("F", genre);
    }

    [Fact]
    public void ExplicitConversion_ValidCode_ReturnsGenre()
    {
        Genre genre = (Genre)"F";

        Assert.Equal(Genre.Fiction, genre);
    }

    [Fact]
    public void FromCode_InvalidCode_ThrowsUnsupportedGenreException()
    {
        Assert.Throws<UnsupportedGenreException>(() => Genre.FromCode("InvalidCode"));
    }

    [Fact]
    public void Fiction_ReturnsGenreWithCodeF()
    {
        Genre genre = Genre.Fiction;
        Assert.Equal("F", genre.Code);
    }

    [Fact]
    public void NonFiction_ReturnsGenreWithCodeNF()
    {
        Genre genre = Genre.NonFiction;
        Assert.Equal("NF", genre.Code);
    }

    [Fact]
    public void Mystery_ReturnsGenreWithCodeM()
    {
        Genre genre = Genre.Mystery;
        Assert.Equal("M", genre.Code);
    }

    [Fact]
    public void SupportedGenres_ContainsAllDefinedGenres()
    {
        IEnumerable<Genre> supportedGenres = new List<Genre>
        {
            Genre.Fiction,
            Genre.NonFiction,
            Genre.Mystery
        };

        Assert.Contains(Genre.Fiction, supportedGenres);
        Assert.Contains(Genre.NonFiction, supportedGenres);
        Assert.Contains(Genre.Mystery, supportedGenres);
    }
}