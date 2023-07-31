using AutoFixture.Kernel;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.UnitTests.AutoFixture.Extensions;

namespace Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders.Api.Goals;
internal class CreateGoalRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(CreateGoalRequest))
            return new NoSpecimen();
        
        DateTimeInterval dateTimeInterval = context.Resolve<DateTimeInterval>();

        return new CreateGoalRequest
        (
            Title: context.Resolve<string>(),
            Description: context.Resolve<string>(),
            AimDateStart: dateTimeInterval.Start,
            AimDateEnd: dateTimeInterval.End
        );       
    }
}
