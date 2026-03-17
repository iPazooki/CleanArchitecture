namespace Domain.UnitTests;

public class GenreTests
{
    [Theory]
    [InlineData("F")]
    [InlineData("NF")]
    [InlineData("M")]
    public void FromCode_Valid_ReturnsExpectedGenre(string code)
    {
        Result<Genre> result = Genre.FromCode(code);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(code, result.Value!.Code);
    }

    [Fact]
    public void FromCode_Invalid_ReturnsFailure()
    {
        Result<Genre> result = Genre.FromCode("XYZ");

        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("f")]
    [InlineData("nf")]
    [InlineData("m")]
    public void FromCode_CaseSensitive_ReturnsFailure(string code)
    {
        Result<Genre> result = Genre.FromCode(code);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Equals_SameCode_ReturnsTrue()
    {
        Genre first = Genre.FromCode("F").Value!;
        Genre second = Genre.FromCode("F").Value!;

        Assert.Equal(first, second);
    }

    [Fact]
    public void Equals_DifferentCode_ReturnsFalse()
    {
        Genre first = Genre.FromCode("F").Value!;
        Genre second = Genre.FromCode("NF").Value!;

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        Assert.Equal("F", Genre.Fiction.ToString());
    }
}
