using AutoFixture.AutoMoq;
using AutoMapper;
using Vita.Goals.Api.Profiles.Authorization;
using Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders;
using Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Api;
using Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Domain;

namespace Vita.Goals.UnitTests.AutoFixture;
public static class TestsFixture
{
    public static IFixture CreateFixture()
    {
        Fixture fixture = new();

        fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Customizations.Add(new GoalSpecimenBuilder());
        fixture.Customizations.Add(new DateTimeIntervalSpecimenBuilder());

        fixture.AddApiSpecimenCustomizations();

        return fixture;
    }
}
