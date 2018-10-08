using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Composer.Store.Tests.Factory
{
    [TestFixture]
    public class StoreViewModelFactory_GetStoreViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateStoreUrlProvider());
        }

        [TestCase("1234567890", "(123) 456-7890")]
        public void WHEN_PhoneNumber_has_formating_SHOULD_be_formated(string phoneNumber, string formatedPhoneNumber)
        {
            //Arrange
            var store = CreateStoreObject();
            store.FulfillmentLocation.Addresses.First().PhoneNumber = phoneNumber;
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateLocalizationProvider("PhoneNumberFormat", "{0:(###) ###-####}"));

            var storeViewModelFactory = _container.CreateInstance<StoreViewModelFactory>();

            //Act
            var model = storeViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
            {
                Store = store,
                BaseUrl = "baseUrl",
                CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
            });

            //Assert
            model.Address.PhoneNumber.ShouldBeEquivalentTo(formatedPhoneNumber);
        }


        private Mock<IStoreUrlProvider> CreateStoreUrlProvider()
        {
            Mock<IStoreUrlProvider> storeUrlProvider = new Mock<IStoreUrlProvider>();

            storeUrlProvider.Setup(p => p.GetStoreUrl(It.IsAny<GetStoreUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return storeUrlProvider;
        }

        private Mock<ILocalizationProvider> CreateLocalizationProvider(string key, string resultLocalizedString)
        {
            Mock<ILocalizationProvider> provider = new Mock<ILocalizationProvider>();

            provider.Setup(p => p.GetLocalizedString(It.Is<GetLocalizedParam>(lp => lp.Key == key)))
                .Returns(() => resultLocalizedString)
                .Verifiable();

            return provider;
        }

        private static Orckestra.Overture.ServiceModel.Customers.Stores.Store CreateStoreObject()
        {
            var store = new Orckestra.Overture.ServiceModel.Customers.Stores.Store
            {
                Number = "0001",
                FulfillmentLocation = new FulfillmentLocation
                {
                    Id = new Guid(),
                    Addresses = new List<Address>
                    {
                        new Address()
                    }
                }
            };

            return store;
        }

        private static Mock<IViewModelMapper> CreateViewModelMapper()
        {
            Mock<IViewModelMapper> mapperMock = new Mock<IViewModelMapper>(MockBehavior.Strict);

            mapperMock.Setup(
                mapper =>
                    mapper.MapTo<StoreViewModel>(It.IsAny<Orckestra.Overture.ServiceModel.Customers.Stores.Store>(),
                        It.IsAny<CultureInfo>()))
                .Returns(new StoreViewModel())
                .Verifiable();

            mapperMock.Setup(
              mapper =>
                  mapper.MapTo<StoreAddressViewModel>(It.IsAny<Orckestra.Overture.ServiceModel.Address>(),
                      It.IsAny<CultureInfo>()))
              .Returns(new StoreAddressViewModel())
              .Verifiable();

            return mapperMock;
        }

    }
}
