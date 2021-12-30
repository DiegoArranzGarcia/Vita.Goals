using System;
using Vita.Goals.Domain.Aggregates.Categories;
using Xunit;

namespace Vita.Goals.Domain.Tests.Aggregates.Categories
{
    public class CategoryTests
    {
        [Fact]
        public void GivenValidArguments_WhenCreatingCategory_ShouldCreateCategory()
        {
            var category = new Category(CategoryTestsFixture.CorrectName, CategoryTestsFixture.CorrectColor, CategoryTestsFixture.CorrectCreatedBy);

            Assert.NotNull(category);
            Assert.Equal(expected: CategoryTestsFixture.CorrectName, actual: category.Name);
            Assert.Equal(expected: CategoryTestsFixture.CorrectColor, actual: category.Color);
            Assert.Equal(expected: CategoryTestsFixture.CorrectCreatedBy, actual: category.CreatedBy);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenBadName_WhenCreatingCategory_ShouldThrowArgumentException(string badName)
        {
            Assert.ThrowsAny<ArgumentException>(() => new Category(badName, CategoryTestsFixture.CorrectColor, CategoryTestsFixture.CorrectCreatedBy));
        }

        [Fact]
        public void GivenEmptyGuidForCreatedBy_WhenCreatingCategory_ShouldThrowArgumentException()
        {
            Assert.ThrowsAny<ArgumentException>(() => new Category(CategoryTestsFixture.CorrectName, CategoryTestsFixture.CorrectColor, Guid.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenBadName_WhenChangingCategoryName_ShouldThrowArgumentException(string badName)
        {
            var category = CategoryTestsFixture.CreateCategory();

            Assert.ThrowsAny<ArgumentException>(() => category.Name = badName);
        }
    }
}
