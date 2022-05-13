using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class PaymentViewServiceGetPaymentMethodsAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var mapper = ViewModelMapperFactory.CreateFake(typeof(PaymentMethodViewModel).Assembly);

            _container.Use<IViewModelMapper>(mapper);

            var cartViewModelFactory = _container.CreateInstance<CartViewModelFactory>();
            _container.Use<ICartViewModelFactory>(cartViewModelFactory);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            MockLookupService();
            var paymentId = GetRandom.Guid();
            MockGetCart(paymentId);
            MockCartPayments(paymentId);
            MockPaymentProvider();

            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    Id = GetRandom.Guid(),
                    PaymentProviderName = GetRandom.String(32),
                    Enabled = true,
                    Type = PaymentMethodType.Cash
                }
            };

            MockPaymentMethods(paymentMethods);

            var service = _container.CreateInstance<PaymentViewService>();

            //Act
            var result = await service.GetPaymentMethodsAsync(BuildGetPaymentMethodsParam()).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            var paymentMethod = result.PaymentMethods.FirstOrDefault();
            paymentMethod.Should().NotBeNull();
            paymentMethod.Id.ShouldBeEquivalentTo(paymentMethods.First().Id);
            paymentMethod.PaymentProviderName.ShouldBeEquivalentTo(paymentMethods.First().PaymentProviderName);
            paymentMethod.DisplayName.ShouldBeEquivalentTo("TestDisplayName");
            result.ActivePaymentViewModel.Should().BeNull();
            result.PaymentId.Should().NotBeEmpty();
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Return_Only_Active_Methods()
        {
            //Arrange
            MockLookupService();
            var paymentId = GetRandom.Guid();
            MockGetCart(paymentId);
            MockCartPayments(paymentId);
            MockPaymentProvider();

            var activeMethodId = GetRandom.Guid();
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    Id = activeMethodId,
                    DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", GetRandom.String(32)}}),
                    PaymentProviderName = GetRandom.String(32),
                    Enabled = true,
                    Type = PaymentMethodType.Cash
                },
                 new PaymentMethod
                {
                    Id = GetRandom.Guid(),
                    DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", GetRandom.String(32)}}),
                    PaymentProviderName = GetRandom.String(32),
                    Enabled = false,
                    Type = PaymentMethodType.Cash
                }
            };

            MockPaymentMethods(paymentMethods);

            var service = _container.CreateInstance<PaymentViewService>();

            //Act
            var result = await service.GetPaymentMethodsAsync(BuildGetPaymentMethodsParam()).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.PaymentMethods.Count.ShouldBeEquivalentTo(1);
            var paymentMethod = result.PaymentMethods.FirstOrDefault();
            paymentMethod.Should().NotBeNull();
            paymentMethod.Id.ShouldBeEquivalentTo(activeMethodId);
            paymentMethod.DisplayName.ShouldBeEquivalentTo("TestDisplayName");

            result.ActivePaymentViewModel.Should().BeNull();
            result.PaymentId.Should().NotBeEmpty();
        }

        [Test]
        public async Task WHEN_payment_method_is_selected_SHOULD_return_ActivePaymentViewModel()
        {
            //Arrange
            MockLookupService();
            MockPaymentProvider();

            var activeMethod = new PaymentMethod
            {
                Id = GetRandom.Guid(),
                DisplayName = new LocalizedString(new Dictionary<string, string> {{"en-US", GetRandom.String(32)}}),
                PaymentProviderName = GetRandom.String(32),
                Enabled = true,
                Type = PaymentMethodType.Cash
            };
            var paymentMethods = new List<PaymentMethod>
            {
                activeMethod
            };

            MockPaymentMethods(paymentMethods);
            MockGetCart(GetRandom.Guid(), activeMethod);

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var vm = await sut.GetPaymentMethodsAsync(new GetPaymentMethodsParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.GetCultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(7),
                ProviderNames = new List<string>()
                {
                    activeMethod.PaymentProviderName
                }
            });

            //Assert
            vm.Should().NotBeNull();
            vm.PaymentId.Should().HaveValue();
            vm.PaymentId.Should().NotBeEmpty();
            vm.PaymentMethods.Should().NotBeNullOrEmpty();
            vm.PaymentMethods.Should().Contain(model => model.Id == activeMethod.Id && model.IsSelected);
            vm.ActivePaymentViewModel.Should().NotBeNull();
            vm.ActivePaymentViewModel.Id.Should().Be(vm.PaymentId.GetValueOrDefault());
            vm.ActivePaymentViewModel.PaymentStatus.Should().NotBeNullOrWhiteSpace();
            vm.ActivePaymentViewModel.ProviderType.Should().NotBeNullOrWhiteSpace();

        }

        private void MockLookupService()
        {
            var mockedLookupService = _container.GetMock<ILookupService>();

            var paymentMethodDisplayNames = new Dictionary<string, string> { { "Cash", "TestDisplayName" } };
            mockedLookupService.Setup(a => a.GetLookupDisplayNamesAsync(It.IsAny<GetLookupDisplayNamesParam>())).ReturnsAsync(
                paymentMethodDisplayNames);
        }

        private void MockPaymentMethods(List<PaymentMethod> availableMethods)
        {
            var mockedPaymentMethodRepository = _container.GetMock<IPaymentRepository>();

            mockedPaymentMethodRepository.Setup(r => r.GetPaymentMethodsAsync(It.IsAny<GetPaymentMethodsParam>()))
                .ReturnsAsync(availableMethods);
        }

        private void MockCartPayments(Guid paymentId)
        {
            var mockedPaymentMethodRepository = _container.GetMock<IPaymentRepository>();
            mockedPaymentMethodRepository.Setup(pr => pr.GetCartPaymentsAsync(It.IsNotNull<GetCartPaymentsParam>()))
                .ReturnsAsync(new List<Payment>() {
                    new Payment
                    {
                        Id = paymentId,
                        Amount = 0.0m,
                        BillingAddress = new Address()
                        {
                            City = GetRandom.String(18),
                            CountryCode = GetRandom.String(2, true),
                            FirstName = GetRandom.String(8),
                            LastName = GetRandom.String(12),
                            IsPreferredBilling = GetRandom.Boolean(),
                            IsPreferredShipping = GetRandom.Boolean(),
                            Email = GetRandom.Email(),
                            Line1 = GetRandom.Phrase(20),
                            Line2 = string.Empty,
                            PhoneNumber = GetRandom.Usa.PhoneNumber(),
                            PostalCode = GetRandom.String(7, true),
                            RegionCode = GetRandom.String(5, true)
                        },
                        PaymentStatus = PaymentStatus.New,
                    }
                });
        }

        private void MockGetCart(Guid paymentId,  PaymentMethod activePaymentMethod = null)
        {
            var cartRepoMock = _container.GetMock<ICartRepository>();
            cartRepoMock.Setup(repo => repo.GetCartAsync(It.IsAny<GetCartParam>()))
                .Returns((GetCartParam p) =>
                {
                    var cart = new ProcessedCart()
                    {
                        ScopeId = p.Scope,
                        CustomerId = p.CustomerId,
                        CultureName = p.CultureInfo.Name,
                        Name = p.CartName,
                        Payments = new List<Payment>
                        {
                            new Payment
                            {
                                Id = paymentId,
                                PaymentStatus = PaymentStatus.New,
                                BillingAddress = new Address(),
                                PaymentMethod = activePaymentMethod
                            }
                        }
                    };

                    return Task.FromResult(cart);
                });
        }

        private void MockPaymentProvider()
        {
            var mockPaymentProviderFactory = _container.GetMock<IPaymentProviderFactory>();

            mockPaymentProviderFactory.Setup(factory => factory.ResolveProvider(It.IsNotNull<string>()))
                .Returns(new FakePaymentProvider());
        }

        private GetPaymentMethodsParam BuildGetPaymentMethodsParam()
        {
            return new GetPaymentMethodsParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                ProviderNames = new List<string> { GetRandom.String(7), GetRandom.String(7) },
                CartName = GetRandom.String(32),
                CustomerId = GetRandom.Guid()
            };
        }
    }
}
