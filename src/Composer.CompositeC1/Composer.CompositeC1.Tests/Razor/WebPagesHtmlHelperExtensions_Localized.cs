using System.Web.WebPages.Html;
using Composite.AspNet.Razor;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Tests.Mocks;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Providers.Membership;

// ReSharper disable InconsistentNaming
namespace Orckestra.Composer.CompositeC1.Tests.Razor
{
    [TestFixture]
    public class WebPagesHtmlHelperExtensions_Localized
    {
        private Mock<ILocalizationProvider> _mockLocalizationProvider;
        private ComposerHostMoq _composerHostMoq;
        private AutofacDependencyResolverMoq _autofacDependencyResolverMoq;


        [SetUp]
        public void Setup()
        {
            _mockLocalizationProvider = new Mock<ILocalizationProvider>();
            _composerHostMoq = new ComposerHostMoq();
            _composerHostMoq.AutoMock.Provide(_mockLocalizationProvider.Object);
           _autofacDependencyResolverMoq = new AutofacDependencyResolverMoq();
        }


        [TearDown]
        public void TearDown()
        {
            _composerHostMoq.Dispose();
            _autofacDependencyResolverMoq.Dispose();
        }


        [Test]
        public void When_LocalizeValue_Exists_Should_Return_Formatted_Value()
        {
            // arrange
            var stringFormat = "Selected {0} from {1}";
            _mockLocalizationProvider.Setup(x => x.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns("Selected {0} from {1}");

            // act
            var result = ((HtmlHelper) null).Localized("Category","Selected", 1, 2);
            // assert

            Assert.IsNotNull(result);
            Assert.AreEqual(result, string.Format(stringFormat, 1, 2));
        }


        [Test]
        public void When_LocalizeValue_Is_NULL_Should_Return_Category_Name_And_Values()
        {
            // arrange
            _mockLocalizationProvider.Setup(x => x.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns(default(string));

            // act
            var result = ((HtmlHelper) null).Localized("Category", "Selected", 1, 2);
            
            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, "[1, 2, Category/Selected]");
        }
    };
}
