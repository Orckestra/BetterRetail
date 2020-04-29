using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Services;

namespace Orckestra.Composer.Store.Tests.Services
{
    [TestFixture]
    public class StoreViewService_GetStoreViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateStoreUrlProvider());
        }

        [Test]
        public void WHEN_GetStoreParam_Is_Null_SHOULD_Throw_Argument_Null_Exception()
        {
            //Arrange
            var storeViewService = _container.CreateInstance<StoreViewService>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await storeViewService.GetStoreViewModelAsync(null);
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentNullException>();
        }

        [TestCase(null, "Canada", "http://foo.com")]
        [TestCase("0001", null, "http://foo.com")]
        [TestCase("0001", "Canada", null)]
        [TestCase(null, null, null)]
        public void WHEN_SomeParam_Is_Null_SHOULD_Throw_Argument_Exception(string storeNumber, string scope,
            string baseUrl)
        {
            //Arrange
            var storeViewService = _container.CreateInstance<StoreViewService>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await storeViewService.GetStoreViewModelAsync(new GetStoreByNumberParam
                {
                    StoreNumber = storeNumber,
                    Scope = scope,
                    BaseUrl = baseUrl,
                    CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
                });
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentException>();
        }

        private Mock<IStoreUrlProvider> CreateStoreUrlProvider()
        {
            Mock<IStoreUrlProvider> storeUrlProvider = new Mock<IStoreUrlProvider>();

            storeUrlProvider.Setup(p => p.GetStoreUrl(It.IsAny<GetStoreUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return storeUrlProvider;
        }

    }
}
