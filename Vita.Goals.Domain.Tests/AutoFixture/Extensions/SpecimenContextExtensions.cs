using AutoFixture.Kernel;

namespace Vita.Goals.UnitTests.AutoFixture.Extensions;
public static class SpecimenContextExtensions
{
    public static T Resolve<T>(this ISpecimenContext context)
    {
        object resolvedValue = context.Resolve(typeof(T));
        return (T)resolvedValue;
    }
}
