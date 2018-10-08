using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Tests.ViewModels.ViewModelMapper;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class BaseViewModelContext
    {
        [Test]
        public void WHEN_Context_Is_Set_SHOULD_Return_Valid_JsonContext()
        {
            // Arrange
            var categoryId = GetRandom.String(5);
            var tags = new[] {GetRandom.String(5), GetRandom.String(5), GetRandom.String(5), GetRandom.String(5)};
            var vm = new TestCategoryViewModel();

            // Act
            vm.Context["CategoryId"] = categoryId;
            vm.Context["TagIds"] = tags;
            var jsonContext = vm.JsonContext;

            // Assert
            var expectedTagIdsInJson = string.Join(",", tags.Select(tag => string.Format("\"{0}\"", tag)));
            var expectedJsonContext = string.Format("{{\"CategoryId\":\"{0}\",\"TagIds\":[{1}]}}", categoryId, expectedTagIdsInJson);
            jsonContext.Should().Be(expectedJsonContext);
        }
    }
}
