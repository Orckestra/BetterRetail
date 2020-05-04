using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Country;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Administration;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CheckoutServiceUpdateCartAsync
    {
        private AutoMocker _container;
        protected ViewModelMapper ViewModelMapper;
        protected Mock<IViewModelMetadataRegistry> MetadataRegistry;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            ConfigureMetadataRegistryForSerialization();

            var paymentProviderFactoryMock = CreatepaymentProviderFactoryMock();
            _container.Use(paymentProviderFactoryMock);

            var countryService = CreateCountryServiceMock();
            _container.Use(countryService);
        }

        private Mock<ILineItemService> CreatepaymentProviderFactoryMock()
        {
            var paymentProviderFactoryMock = _container.GetMock<ILineItemService>();
            return paymentProviderFactoryMock;
        }

        private static Mock<ICountryService> CreateCountryServiceMock()
        {
            var country = new Country.CountryViewModel();

            var countryService = new Mock<ICountryService>(MockBehavior.Strict);

            countryService.Setup(c => c.RetrieveCountryAsync(It.IsAny<RetrieveCountryParam>()))
                .ReturnsAsync(country)
                .Verifiable();

            countryService.Setup(c => c.RetrieveRegionDisplayNameAsync(It.IsAny<RetrieveRegionDisplayNameParam>()))
                .ReturnsAsync(GetRandom.String(32))
                .Verifiable();
            return countryService;
        }

        private void ConfigureMetadataRegistryForSerialization()
        {
            MetadataRegistry = _container.GetMock<IViewModelMetadataRegistry>();

            ConfigureViewModelMetadata(typeof(CustomerSummaryViewModel), typeof(ICustomCustomerViewModel));
            ConfigureViewModelMetadata(typeof(AddressViewModel));
            ConfigureViewModelMetadata(typeof(ShippingMethodViewModel));
            ConfigureViewModelMetadata(typeof(CartViewModel));
            ConfigureViewModelMetadata(typeof(PaymentMethodViewModel));
            ConfigureViewModelMetadata(typeof(PaymentViewModel));
            ConfigureViewModelMetadata(typeof(BillingAddressViewModel));
            ConfigureViewModelMetadata(typeof(RegisteredShippingAddressViewModel));

            ViewModelMetadataRegistry.Current = MetadataRegistry.Object;
            ViewModelMapper = new ViewModelMapper(MetadataRegistry.Object,
                _container.GetMock<IViewModelPropertyFormatter>().Object, _container.GetMock<ILookupService>().Object, _container.GetMock<ILocalizationProvider>().Object);
        }

        private void ConfigureViewModelMetadata(Type viewModelType, Type extendedModelType = null)
        {
            var viewModelMetadata = new List<IPropertyMetadata>();

            foreach (var propertyInfo in viewModelType.GetProperties())
            {
                viewModelMetadata.Add(new InstancePropertyMetadata(propertyInfo));
            }
            if (extendedModelType != null)
            {
                foreach (var propertyInfo in extendedModelType.GetProperties())
                {
                    viewModelMetadata.Add(new BagPropertyMetadata(propertyInfo));
                }
            }
            MetadataRegistry.Setup(m => m.GetViewModelMetadata(viewModelType)).Returns(viewModelMetadata);
        }

        [Test]
        public void WHEN_Passing_Null_Parameter_SHOULD_Throw_Exception()
        {
            //Arrange
            var service = _container.CreateInstance<CheckoutService>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateCheckoutCartAsync(null));
        }

        [Test]
        public void WHEN_Passing_Null_UpdateValues_Parameter_SHOULD_Throw_Exception()
        {
            //Arrange
            var service = _container.CreateInstance<CheckoutService>();
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = new GetCartParam(),
                CurrentStep = GetRandom.Int(),
                UpdateValues = null,
                IsGuest = GetRandom.Boolean()
            };

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCheckoutCartAsync(param));
        }

        [Test]
        public void WHEN_Passing_Null_GetCartParam_Parameter_SHOULD_Throw_Exception()
        {
            //Arrange
            var service = _container.CreateInstance<CheckoutService>();
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = null,
                CurrentStep = GetRandom.Int(),
                UpdateValues = new Dictionary<string, string>(),
                IsGuest = GetRandom.Boolean()
            };

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCheckoutCartAsync(param));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void WHEN_Registering_Operation_With_Null_OperatiomName_SHOULD_Throw_Exception(string operationName)
        {
            //Arrange
            var service = _container.CreateInstance<CheckoutService>();

            // Act and Assert
            Assert.Throws<ArgumentException>(() => service.RegisterCartUpdateOperation<CustomerSummaryViewModel>(operationName, UpdateCustomerCustom, GetRandom.PositiveInt(100)));
        }

        [Test]
        public void WHEN_Registering_Operation_With_Null_Action_SHOULD_Throw_Exception()
        {
            //Arrange
            var service = _container.CreateInstance<CheckoutService>();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => service.RegisterCartUpdateOperation<CustomerSummaryViewModel>("test", null, GetRandom.PositiveInt(100)));
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Update_GuestCustomerInfo()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Customer.Email = GetRandom.Email();

            var service = CreateCheckoutService(cart);

            var updatedEmail = GetRandom.Email();
            var updatedCustomer = new CustomerSummaryViewModel
            {
                Email = updatedEmail
            };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("GuestCustomerInfo", updatedCustomer),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };

            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.Customer.Email.ShouldBeEquivalentTo(updatedEmail);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Update_ShippingAddress()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Shipments.First().Address = new Address()
            {
                City = GetRandom.String(10),
                PostalCode = GetRandom.String(6)
            };

            var service = CreateCheckoutService(cart);

            var updatedAddress = new AddressViewModel
            {
                City = GetRandom.String(32),
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                Line1 = GetRandom.String(32),
                Line2 = GetRandom.String(32),
                RegionCode = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                PhoneNumber = GetRandom.String(32),
                PostalCode = GetRandom.String(6)
            };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("ShippingAddress", updatedAddress),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Cart.Should().NotBeNull();
            processedCart.Cart.ShippingAddress.City.ShouldBeEquivalentTo(updatedAddress.City);
            processedCart.Cart.ShippingAddress.FirstName.ShouldBeEquivalentTo(updatedAddress.FirstName);
            processedCart.Cart.ShippingAddress.LastName.ShouldBeEquivalentTo(updatedAddress.LastName);
            processedCart.Cart.ShippingAddress.Line1.ShouldBeEquivalentTo(updatedAddress.Line1);
            processedCart.Cart.ShippingAddress.Line2.ShouldBeEquivalentTo(updatedAddress.Line2);
            processedCart.Cart.ShippingAddress.RegionCode.ShouldBeEquivalentTo(updatedAddress.RegionCode);
            processedCart.Cart.ShippingAddress.CountryCode.ShouldBeEquivalentTo(updatedAddress.CountryCode);
            processedCart.Cart.ShippingAddress.PhoneNumber.ShouldBeEquivalentTo(updatedAddress.PhoneNumber);
        }

        [Test]
        public async Task WHEN_Passing_RegisteredCustomer_Valid_Parameters_SHOULD_Update_ShippingAddress()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Shipments.First().Address = new Address()
            {
                City = GetRandom.String(10),
                PostalCode = GetRandom.String(6),
                Id = Guid.NewGuid()
            };

            var service = CreateCheckoutService(cart);

            var updatedAddress = new RegisteredShippingAddressViewModel
            {
                ShippingAddressId = Guid.NewGuid()
            };
            var address = await _container.Get<IAddressRepository>().GetAddressByIdAsync(updatedAddress.ShippingAddressId);

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("ShippingAddressRegistered", updatedAddress),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Cart.Should().NotBeNull();
            processedCart.Cart.ShippingAddress.AddressBookId.ShouldBeEquivalentTo(updatedAddress.ShippingAddressId);
            processedCart.Cart.ShippingAddress.AddressName.ShouldBeEquivalentTo(address.AddressName);
            processedCart.Cart.ShippingAddress.City.ShouldBeEquivalentTo(address.City);
            processedCart.Cart.ShippingAddress.CountryCode.ShouldBeEquivalentTo(address.CountryCode);
            processedCart.Cart.ShippingAddress.FirstName.ShouldBeEquivalentTo(address.FirstName);
            processedCart.Cart.ShippingAddress.LastName.ShouldBeEquivalentTo(address.LastName);
            processedCart.Cart.ShippingAddress.Line1.ShouldBeEquivalentTo(address.Line1);
            processedCart.Cart.ShippingAddress.Line2.ShouldBeEquivalentTo(address.Line2);
            processedCart.Cart.ShippingAddress.PhoneNumber.ShouldBeEquivalentTo(address.PhoneNumber);
            processedCart.Cart.ShippingAddress.PostalCode.ShouldBeEquivalentTo(address.PostalCode);
            processedCart.Cart.ShippingAddress.RegionCode.ShouldBeEquivalentTo(address.RegionCode);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Update_ShippingMethod()
        {
            //Arrange
            var updatedShippingMethod = new ShippingMethodViewModel
            {
                Name = GetRandom.String(32),
                ShippingProviderId = Guid.NewGuid(),
                Cost = GetRandom.PositiveInt(100).ToString(),
            };

            var mock = new Mock<IFulfillmentMethodRepository>();
            mock.Setup(repo => repo.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                .ReturnsAsync(new List<FulfillmentMethod>
                {
                    new FulfillmentMethod(),
                    new FulfillmentMethod
                    {
                        Name = updatedShippingMethod.Name,
                        ShippingProviderId = updatedShippingMethod.ShippingProviderId
                    }
                });

            _container.Use(mock);

            var cart = CreateBasicCart();
            var service = CreateCheckoutService(cart);

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("ShippingMethod", updatedShippingMethod),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Cart.Should().NotBeNull();
            processedCart.Cart.ShippingMethod.Name.Should().BeEquivalentTo(updatedShippingMethod.Name);
            processedCart.Cart.ShippingMethod.ShippingProviderId.ShouldBeEquivalentTo(updatedShippingMethod.ShippingProviderId);
        }

        [Ignore("Found the test with this attribute, filling in required reason.")]
        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Update_BillingAddress()
        {
            //Arrange
            var cart = CreateBasicCart();
            var service = CreateCheckoutService(cart);

            var updatedBillingAddress = new BillingAddressViewModel
            {
                UseShippingAddress = false,
                City = "Paris",
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                Line1 = GetRandom.String(32),
                Line2 = GetRandom.String(32),
                RegionCode = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                PhoneNumber = GetRandom.String(32),
            };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("BillingAddress", updatedBillingAddress),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.Payment.BillingAddress.City.Should().Be("Paris");
            processedCart.Cart.Payment.BillingAddress.FirstName.ShouldBeEquivalentTo(updatedBillingAddress.FirstName);
            processedCart.Cart.Payment.BillingAddress.LastName.ShouldBeEquivalentTo(updatedBillingAddress.LastName);
            processedCart.Cart.Payment.BillingAddress.Line1.ShouldBeEquivalentTo(updatedBillingAddress.Line1);
            processedCart.Cart.Payment.BillingAddress.Line2.ShouldBeEquivalentTo(updatedBillingAddress.Line2);
            processedCart.Cart.Payment.BillingAddress.RegionCode.ShouldBeEquivalentTo(updatedBillingAddress.RegionCode);
            processedCart.Cart.Payment.BillingAddress.CountryCode.ShouldBeEquivalentTo(updatedBillingAddress.CountryCode);
            processedCart.Cart.Payment.BillingAddress.PhoneNumber.ShouldBeEquivalentTo(updatedBillingAddress.PhoneNumber);
        }

        [Ignore("Found the test with this attribute, filling in required reason.")]
        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Update_BillingAddress_With_ShippingAddress()
        {
            //Arrange
            var shippingAddress = new Address
            {
                City = "Paris",
                FirstName = GetRandom.String(32),
                LastName = GetRandom.String(32),
                Line1 = GetRandom.String(32),
                Line2 = GetRandom.String(32),
                RegionCode = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                PhoneNumber = GetRandom.String(32)
            };

            var cart = CreateBasicCart();
            cart.Shipments.First().Address = shippingAddress;

            var service = CreateCheckoutService(cart);

            var updatedBillingAddress = new BillingAddressViewModel
            {
                UseShippingAddress = true,
            };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("BillingAddress", updatedBillingAddress),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.Payment.BillingAddress.City.Should().Be("Paris");
            processedCart.Cart.Payment.BillingAddress.FirstName.ShouldBeEquivalentTo(shippingAddress.FirstName);
            processedCart.Cart.Payment.BillingAddress.LastName.ShouldBeEquivalentTo(shippingAddress.LastName);
            processedCart.Cart.Payment.BillingAddress.Line1.ShouldBeEquivalentTo(shippingAddress.Line1);
            processedCart.Cart.Payment.BillingAddress.Line2.ShouldBeEquivalentTo(shippingAddress.Line2);
            processedCart.Cart.Payment.BillingAddress.RegionCode.ShouldBeEquivalentTo(shippingAddress.RegionCode);
            processedCart.Cart.Payment.BillingAddress.CountryCode.ShouldBeEquivalentTo(shippingAddress.CountryCode);
            processedCart.Cart.Payment.BillingAddress.PhoneNumber.ShouldBeEquivalentTo(shippingAddress.PhoneNumber);
        }

        [Test]
        public async Task WHEN_CustomMethod_Registered_AND_Passing_Valid_Parameters_SHOULD_Update_GuestCustomerInfo()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Customer.Email = GetRandom.Email();

            var service = CreateCheckoutService(cart);

            var updatedCustomer = new CustomerSummaryViewModel
            {
                Email = GetRandom.Email()
            };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = CreateUpdateOperation("GuestCustomerInfo", updatedCustomer),
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.Customer.Email.ShouldBeEquivalentTo(updatedCustomer.Email);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_And_Having_Custom_Field_SHOULD_Update_GuestCustomerInfo()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Customer.Email = GetRandom.Email();

            var service = CreateCheckoutService(cart);
            service.RegisterCartUpdateOperation<CustomerSummaryViewModel>("GuestCustomerInfo", UpdateCustomerWithCustomFieldInBag, GetRandom.PositiveInt(100));

            var updatedCustomer = new FakeCustomerViewModelWithCustomField
            {
                Email = "test2",
                CustomField = "test3"
            };

            var jsonViewModel = JsonConvert.SerializeObject(updatedCustomer);
            var operations = new Dictionary<string, string> { { "GuestCustomerInfo", jsonViewModel } };

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = operations,
                CurrentStep = GetRandom.Int(),
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.Customer.Email.ShouldBeEquivalentTo("test2-test3");
        }

        [Test]
        public async Task WHEN_Passing_Valid_Paramaters_SHould_Update_LastCheckoutStep()
        {
            //Arrange
            var cart = CreateBasicCart();
            cart.Shipments = new List<Shipment>
            {
                new Shipment
                {
                    LineItems = new List<LineItem>
                    {
                        new LineItem()
                    }
                }
            };

            var service = CreateCheckoutService(cart);

            // Act
            var param = new UpdateCheckoutCartParam
            {
                GetCartParam = CreateGetCartParam(),
                UpdateValues = new Dictionary<string, string>(),
                CurrentStep = 2,
                IsGuest = GetRandom.Boolean()
            };
            var processedCart = await service.UpdateCheckoutCartAsync(param);

            //Assert
            processedCart.Should().NotBeNull();
            processedCart.Cart.OrderSummary.CheckoutRedirectAction.LastCheckoutStep.ShouldBeEquivalentTo(3);
        }

        private Task UpdateCustomerCustom(Overture.ServiceModel.Orders.Cart cart, CustomerSummaryViewModel value)
        {
            if (value == null)
            {
                return Task.FromResult(0);
            }

            if (cart.Customer == null)
            {
                cart.Customer = new CustomerSummary();
            }

            cart.Customer.Email = value.Email;

            return Task.FromResult(0);
        }

        private Task UpdateCustomerWithCustomFieldInBag(Overture.ServiceModel.Orders.Cart cart, CustomerSummaryViewModel value)
        {
            if (value == null)
            {
                return Task.FromResult(0);
            }

            if (cart.Customer == null)
            {
                cart.Customer = new CustomerSummary();
            }

            cart.Customer.Email = value.Email + "-" + value.Bag["CustomField"];
            return Task.FromResult(0);
        }

        private static Dictionary<string, string> CreateUpdateOperation(string operationName, object updateViewModel)
        {
            var jsonViewModel = JsonConvert.SerializeObject(updateViewModel);
            var operations = new Dictionary<string, string> { { operationName, jsonViewModel } };

            return operations;
        }

        private CheckoutService CreateCheckoutService(ProcessedCart cart)
        {
            var mockedLookupService = new Mock<ILookupService>();
            var mockedImageService = new Mock<IImageService>();

            mockedLookupService.Setup(a => a.GetLookupDisplayNamesAsync(It.IsAny<GetLookupDisplayNamesParam>())).ReturnsAsync(
                new Dictionary<string, string> { { "Cash", "TestDisplayName" } });

            mockedImageService.Setup(a => a.GetImageUrlsAsync(It.IsAny<IEnumerable<LineItem>>())).ReturnsAsync(new List<ProductMainImage>());

            _container.Use(mockedImageService);
            _container.Use(mockedLookupService);

            var mockedDamProvider = new Mock<IDamProvider>();
            mockedDamProvider.Setup(x => x.GetProductMainImagesAsync(It.IsAny<GetProductMainImagesParam>()))
                .ReturnsAsync(new List<ProductMainImage>());

            _container.Use(mockedDamProvider);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            var address = new Address()
            {
                PropertyBag = new PropertyBag(),
                FirstName = GetRandom.String(5),
                LastName = GetRandom.String(5),
                AddressName = GetRandom.String(5),
                Line2 = GetRandom.String(5),
                Line1 = GetRandom.String(5),
                CountryCode = GetRandom.String(3),
                City = GetRandom.String(5),
                PostalCode = GetRandom.String(5),
                PhoneNumber = GetRandom.String(5),
                RegionCode = GetRandom.String(5),
                Email = GetRandom.String(5),
                PhoneExtension = GetRandom.String(5),
            };
            mockedAddressRepository.Setup(x => x.GetAddressByIdAsync(It.IsAny<Guid>()))
                .Returns((Guid id) =>
                {
                    var result = address;
                    result.Id = id;
                    return Task.FromResult(result);
                });
            _container.Use(mockedAddressRepository);

            _container.Use<ILocalizationProvider>(LocalizationProviderFactory.Create());
            _container.Use<IViewModelMapper>(ViewModelMapper);
            _container.Use<ICartRepository>(new CartRepositoryUpdateCartMock { CurrentCart = cart });
            _container.Use<IComposerJsonSerializer>(new ComposerJsonSerializerMock(MetadataRegistry.Object, ViewModelMapper));
            var cartViewModelFactory = _container.CreateInstance<CartViewModelFactory>();
            _container.Use<ICartViewModelFactory>(cartViewModelFactory);
            return _container.CreateInstance<CheckoutService>();
        }

        private static GetCartParam CreateGetCartParam()
        {
            return new GetCartParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.Url()
            };
        }

        private static ProcessedCart CreateBasicCart()
        {
            var cart = new ProcessedCart
            {
                CultureName = "en-US",
                Shipments = new List<Shipment>
                {
                    new Shipment()
                    {
                        Id = GetRandom.Guid()
                    }
                },
                Payments = new List<Payment>()
                {
                    new Payment()
                    {
                        PaymentStatus = PaymentStatus.New
                    }
                },
                Total = 10,
                Customer = new CustomerSummary()
            };

            return cart;
        }
    }

    public interface ICustomCustomerViewModel : IExtensionOf<CustomerSummaryViewModel>
    {
        string CustomField { get; set; }
    }

    class FakeCustomerViewModelWithCustomField
    {
        public string Email { get; set; }
        public string CustomField { get; set; }
    }


}
