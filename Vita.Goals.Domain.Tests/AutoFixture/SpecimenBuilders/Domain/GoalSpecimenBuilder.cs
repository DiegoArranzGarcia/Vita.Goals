using AutoFixture.Kernel;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.AutoFixture.Extensions;

namespace Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Domain;
public class GoalSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var requestAsType = request as Type;
        if (typeof(Goal).Equals(requestAsType))
        {
            return new Goal
            (
                title: context.Resolve<string>(),
                createdBy: context.Resolve<Guid>()
            );
        }

        return new NoSpecimen();
    }
}
