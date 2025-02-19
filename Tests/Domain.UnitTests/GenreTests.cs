namespace Domain.UnitTests;

public class GenreTests
{
    [Theory]
    [InlineData("F")]
    [InlineData("NF")]
    [InlineData("M")]
    public void FromCode_Valid_ReturnsExpectedGenre(string code)
    {
        Genre genre = Genre.FromCode(code);
        Assert.NotNull(genre);
        Assert.Equal(code, genre.Code);
    }

    [Fact]
    public void FromCode_Invalid_ThrowsUnsupportedGenreException()
    {
        Assert.Throws<UnsupportedGenreException>(() => Genre.FromCode("XYZ"));
    }

    [Theory]
    [InlineData("f")]
    [InlineData("nf")]
    [InlineData("m")]
    public void FromCode_CaseSensitive_ThrowsUnsupportedGenreException(string code)
    {
        Assert.Throws<UnsupportedGenreException>(() => Genre.FromCode(code));
    }

    [Fact]
    public void Equals_SameCode_ReturnsTrue()
    {
        Genre first = Genre.FromCode("F");
        Genre second = Genre.FromCode("F");
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equals_DifferentCode_ReturnsFalse()
    {
        Genre first = Genre.FromCode("F");
        Genre second = Genre.FromCode("NF");
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        Assert.Equal("F", Genre.Fiction.ToString());
    }
}