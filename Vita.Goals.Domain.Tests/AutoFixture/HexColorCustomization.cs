using AutoFixture;
using AutoFixture.Kernel;
using System;
using System.Linq;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Tests.AutoFixture
{
    public class HexColorCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new HexColorSpecimenBuilder());
        }
    }

    public class HexColorSpecimenBuilder : ISpecimenBuilder
    {
        private readonly Random _rng = new();
        private const string _chars = "ABCDEF0123456789";

        public object Create(object request, ISpecimenContext context)
        {
            var requestAsType = request as Type;
            if (typeof(HexColor).Equals(requestAsType))
            {
                string hexCode = new(Enumerable.Range(start: 0, count: 6)
                                               .Select(x => _chars[_rng.Next(_chars.Length)])
                                               .ToArray());

                return new HexColor($"#{hexCode}");
            }

            return new NoSpecimen();
        }
    }
}
