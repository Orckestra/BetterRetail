using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Customers.Stores;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Store.Tests.Services
{
    [TestFixture]
    public class StoreLocatorViewService_GetStoreLocatorViewModelAsync
    {
        private AutoMocker _container;
        private readonly string[] StoresNumbers = new[] {"0001", "0002", "0003", "0004"};

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateStoreUrlProvider());
            _container.Use(CreateStoreRepository(StoresNumbers));
            _container.Use(CreateStoreViewModelFactory(StoresNumbers));
        }

        [Test]
        public void WHEN_GetStoreLocatorViewModelParam_Is_Null_SHOULD_Throw_Argument_Null_Exception()
        {
            //Arrange
            var service = _container.CreateInstance<StoreLocatorViewService>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await service.GetStoreLocatorViewModelAsync(null);
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentNullException>();
        }

        [TestCase(null, "http://foo.com")]
        [TestCase(null, "http://foo.com")]
        [TestCase(null, null)]
        public void WHEN_RequiredParams_Is_Null_SHOULD_Throw_Argument_Exception(string scope,
            string baseUrl)
        {
            //Arrange
            var service = _container.CreateInstance<StoreLocatorViewService>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await service.GetStoreLocatorViewModelAsync(new GetStoreLocatorViewModelParam
                {
                    Scope = scope,
                    BaseUrl = baseUrl,
                    CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
                });
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_SearchPoint_SHOULD_Calculate_NearestStoreCoordinate()
        {
            //Arrange
            var service = _container.CreateInstance<StoreLocatorViewService>();
            var param = CreateBaseGetStoreLocatorViewModelParam();
            param.SearchPoint = new Coordinate(45.45, -74.44);

            //Act
            var model = service.GetStoreLocatorViewModelAsync(param).Result;

            //Assert
            model.NearestStoreCoordinate.Should().NotBeNull();

        }

        [Test]
        public void WHEN_SearchPoint_SHOULD_Calculate_SearchIndexes()
        {
            //Arrange
            var service = _container.CreateInstance<StoreLocatorViewService>();
            var param = CreateBaseGetStoreLocatorViewModelParam();
            param.SearchPoint = new Coordinate(45.45, -74.44);

            //Act
            var model = service.GetStoreLocatorViewModelAsync(param).Result;

            //Assert
            model.Stores.First().SearchIndex.ShouldBeEquivalentTo(1);
            model.Stores.Last().SearchIndex.ShouldBeEquivalentTo(model.Stores.Count);
        }


        [Test]
        public void WHEN_SearchPoint_And_SecondPage_SHOULD_SearchIndexes_DoNot_Start_From_One()
        {
            //Arrange
            var service = _container.CreateInstance<StoreLocatorViewService>();
            var param = CreateBaseGetStoreLocatorViewModelParam();
            param.PageSize = 2;
            param.PageNumber = 2;
            param.SearchPoint = new Coordinate(45.45, -74.44);

            //Act
            var model = service.GetStoreLocatorViewModelAsync(param).Result;

            //Assert
            model.Stores.First().SearchIndex.ShouldBeEquivalentTo(param.PageSize + 1);
            model.Stores.Last().SearchIndex.ShouldBeEquivalentTo(model.Stores.Count + param.PageSize);
        }


        private Mock<IStoreRepository> CreateStoreRepository(string[] numbers)
        {
            Mock<IStoreRepository> repo = new Mock<IStoreRepository>();
            repo.Setup(r => r.GetStoresAsync(It.IsAny<GetStoresParam>()))
                .ReturnsAsync(GetStores(numbers));

            return repo;
        }

        private Mock<IStoreViewModelFactory> CreateStoreViewModelFactory(string[] numbers)
        {
            Mock<IStoreViewModelFactory> factory = new Mock<IStoreViewModelFactory>();
            foreach (var number in numbers)
            {
                var store = new StoreViewModel
                {
                    Number = number
                };
                factory.Setup(
                    r => r.CreateStoreViewModel(It.Is<CreateStoreViewModelParam>(param => param.Store.Number == number)))
                    .Returns(store);
            }
            return factory;
        }

        private Mock<IStoreUrlProvider> CreateStoreUrlProvider()
        {
            Mock<IStoreUrlProvider> storeUrlProvider = new Mock<IStoreUrlProvider>();

            storeUrlProvider.Setup(p => p.GetStoreUrl(It.IsAny<GetStoreUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return storeUrlProvider;
        }

        private GetStoreLocatorViewModelParam CreateBaseGetStoreLocatorViewModelParam()
        {
            return new GetStoreLocatorViewModelParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.CreateSpecificCulture("en-CA"),
                BaseUrl = "http://foo.com",
                IncludeMarkers = false,
                PageNumber = 1,
                PageSize = 4
            };
        }

        private FindStoresQueryResult GetStores(string[] numbers)
        {
            var result = new FindStoresQueryResult();
            result.Results = new List<Orckestra.Overture.ServiceModel.Customers.Stores.Store>();
            foreach (var number in numbers)
            {
                result.Results.Add(CreateStore(number));
            }

            return result;
        }

        private Orckestra.Overture.ServiceModel.Customers.Stores.Store CreateStore(string number)
        {
            return new Orckestra.Overture.ServiceModel.Customers.Stores.Store
            {
                Number = number,
                StoreSchedule = new FulfillmentSchedule(),
                FulfillmentLocation = new FulfillmentLocation
                {
                    Id = Guid.NewGuid(),
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            AddressName = "address",
                            City = "City",
                            Email = "email",
                            Latitude = 43.00,
                            Longitude = -70.00
                        }
                    }
                }
            };
        }
    }
}
