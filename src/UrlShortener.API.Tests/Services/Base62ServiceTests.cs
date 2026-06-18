using UrlShortener.Application.Services;

namespace UrlShortener.API.Tests.Services;

public class Base62ServiceTests
{
    private readonly Base62Service _sut = new();

    [Fact]
    public void ToBase62_WithZero_ReturnsZero()
    {
        var result = _sut.ToBase62(0);

        Assert.Equal("0", result);
    }

    [Fact]
    public void ToBase62_With62_Returns10()
    {
        var result = _sut.ToBase62(62);

        Assert.Equal("10", result);
    }

    [Fact]
    public void FromBase62_WithZero_ReturnsZero()
    {
        var result = _sut.FromBase62("0");

        Assert.Equal(0L, result);
    }

    [Fact]
    public void FromBase62_With10_Returns62()
    {
        var result = _sut.FromBase62("10");

        Assert.Equal(62L, result);
    }

    [Fact]
    public void ToBase62_AndFromBase62_AreInverse()
    {
        const long original = 916132199L;

        var encoded = _sut.ToBase62(original);
        var decoded = _sut.FromBase62(encoded);

        Assert.Equal(original, decoded);
    }
}
