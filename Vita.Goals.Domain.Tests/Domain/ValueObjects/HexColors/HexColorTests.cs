using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.UnitTests.Domain.ValueObjects.HexColors;

public class HexColorTests
{
    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    [InlineData("#3F0012")]
    public void GivenValidHexColors_WhenCreatingHexColor_ShouldReturnColor(string hexColorStr)
    {
        var hexColor = new HexColor(hexColorStr);

        hexColor.Color.Should().Be(expected: hexColorStr);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("#FFFFFFF")]
    [InlineData("#FFFFFZ")]
    public void BadValidHexColors_WhenCreatingHexColor_ShouldThrowArgumentException(string badHexColors)
    {
        Action act = () => _ = new HexColor(badHexColors);

        act.Should().Throw<ArgumentException>();
    }
}
