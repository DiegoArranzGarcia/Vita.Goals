using AutoFixture;
using AutoFixture.Kernel;
using System;
using System.Linq;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Tests.AutoFixture;

public class DateTimeIntervalCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new DateTimeIntervalSpecimenBuilder());
    }
}

public class DateTimeIntervalSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var requestAsType = request as Type;
        if (typeof(DateTimeInterval).Equals(requestAsType))
        {
            var times = context.CreateMany<DateTime>(count: 2);
            return new DateTimeInterval(times.Min(), times.Max());
        }

        return new NoSpecimen();
    }
}
