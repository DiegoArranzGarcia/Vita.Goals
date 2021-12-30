using AutoFixture;
using System;
using Vita.Goals.Domain.Aggregates.Categories;
using Vita.Goals.Domain.Tests.AutoFixture;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Tests.Aggregates.Categories
{
    public static class CategoryTestsFixture
    {
        private static readonly IFixture _fixture = new Fixture().Customize(new HexColorCustomization());

        internal static string CorrectName { get; private set; }
        internal static HexColor CorrectColor { get; private set; }
        internal static Guid CorrectCreatedBy { get; private set; }

        static CategoryTestsFixture()
        {
            CorrectName = _fixture.Create<string>();
            CorrectCreatedBy = _fixture.Create<Guid>();
            CorrectColor = _fixture.Create<HexColor>();
        }

        internal static Category CreateCategory()
        {
            return _fixture.Build<Category>()
                           .Without(x => x.Events)
                           .Create();
        }
    }
}
