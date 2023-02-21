using System;
using Vita.Goals.Domain.ValueObjects;
using Xunit;

namespace Vita.Goals.Domain.Tests.ValueObjects.HexColors;

public class HexColorTests
{
    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    [InlineData("#3F0012")]
    public void GivenValidHexColors_WhenCreatingHexColor_ShouldReturnColor(string hexColorStr)
    {
        var hexColor = new HexColor(hexColorStr);

        Assert.NotNull(hexColor);
        Assert.Equal(expected: hexColorStr, hexColor.Color);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("#FFFFFFF")]
    [InlineData("#FFFFFZ")]
    public void BadValidHexColors_WhenCreatingHexColor_ShouldThrowArgumentException(string badHexColors)
    {
        Assert.ThrowsAny<ArgumentException>(() => new HexColor(badHexColors));
    }
}
