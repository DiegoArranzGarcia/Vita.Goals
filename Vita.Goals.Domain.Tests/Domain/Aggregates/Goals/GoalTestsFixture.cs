using Vita.Common;
using Vita.Goals.UnitTests.AutoFixture;

namespace Vita.Goals.UnitTests.Domain.Aggregates.Goals;

public static class GoalTestsFixture
{
    internal static string ValidTitle { get; private set; }
    internal static string ValidDescription { get; private set; }
    internal static Guid ValidCreatedBy { get; private set; }
    internal static DateTimeInterval ValidAimDate { get; private set; }

    static GoalTestsFixture()
    {
        ValidTitle = "Title";
        ValidDescription = "Description";
        ValidCreatedBy = Guid.NewGuid();
    }

    public static IEnumerable<object[]> GetValidArgumentsCombination()
    {
        yield return new object[] { ValidDescription, ValidAimDate };
        yield return new object[] { ValidDescription, null };
        yield return new object[] { null, ValidAimDate };
        yield return new object[] { null, null };
    }

    public static IEnumerable<object[]> GetValidDescriptions()
    {
        yield return new object[] { null };
        yield return new object[] { string.Empty };
        yield return new object[] { TestsFixture.CreateFixture().Create<string>() };
    }

    public static IEnumerable<object[]> GetValidAimDate()
    {
        yield return new object[] { null };
        yield return new object[] { TestsFixture.CreateFixture().Create<DateTimeInterval>() };
    }

}
