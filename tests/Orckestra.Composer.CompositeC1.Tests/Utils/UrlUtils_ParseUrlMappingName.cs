using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Utils;

namespace Orckestra.Composer.CompositeC1.Tests.Utils
{
    [TestFixture]
    public class UrlUtils_ParseUrlMappingName
    {
        public AutoMocker Container { get; set; }

        [TestCase("/en", "en")]
        [TestCase("/en/", "en")]
        [TestCase("/en-CA/stores", "en-CA")]
        [TestCase("/FR/stores", "FR")]
        [TestCase("stores", null)]
        public void WHEN_requested_path_SHOULD_return_expected_result(string requestedPath, string expectedResult)
        {
            //Act
            var urlMappingName = UrlUtils.ParseUrlMappingName(requestedPath);

            //Assert
            urlMappingName.Should().Be(expectedResult);
        }

    }
}
