using Vita.Goals.UnitTests.AutoFixture;

namespace Vita.Goals.UnitTests.Attributes;
public sealed class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(TestsFixture.CreateFixture)
    {

    }
}
