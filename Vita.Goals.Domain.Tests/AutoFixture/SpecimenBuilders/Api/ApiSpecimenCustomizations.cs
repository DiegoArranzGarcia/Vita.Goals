using Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Api.Goals;

namespace Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Api;
public static class ApiSpecimenCustomizations
{
    public static Fixture AddApiSpecimenCustomizations(this Fixture fixture)
    {
        fixture.Customizations.Add(new CreateGoalRequestSpecimenBuilder());

        return fixture;
    }
}
