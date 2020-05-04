using System;
using System.Collections.Generic;
using System.Globalization;
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
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class PaymentViewServiceUpdatePaymentMethodAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var repoMock = _container.GetMock<IPaymentRepository>();
            repoMock.Setup(repo => repo.UpdatePaymentMethodAsync(It.IsNotNull<UpdatePaymentMethodParam>()))
                .Returns((UpdatePaymentMethodParam p) =>
                {
                    var cart = new ProcessedCart
                    {
                        Name = p.CartName,
                        CultureName = p.CultureInfo.Name,
                        ScopeId = p.Scope,
                        CustomerId = p.CustomerId,
                        Payments = new List<Payment>
                        {
                            new Payment
                            {
                                Id = p.PaymentId,
                                PaymentMethod = new PaymentMethod
                                {
                                    Id = p.PaymentMethodId,
                                    PaymentProviderName = p.PaymentProviderName,
                                    Type = GetRandom.Enumeration<PaymentMethodType>(),
                                    DisplayName = new LocalizedString
                                    {
                                        { p.CultureInfo.Name, GetRandom.String(12) }
                                    },
                                    Enabled = true,
                                    Default = GetRandom.Boolean(),
                                },
                                PaymentStatus = PaymentStatus.New
                            }
                        }
                    };

                    return Task.FromResult(cart);
                }).Verifiable("The repository was not called to Update the payment method.");

            repoMock.Setup(repo => repo.InitializePaymentAsync(It.IsNotNull<InitializePaymentParam>()))
                .Returns((InitializePaymentParam p) =>
                {
                    var cart = new Overture.ServiceModel.Orders.Cart
                    {
                        Name = p.CartName,
                        CultureName = p.CultureInfo.Name,
                        ScopeId = p.Scope,
                        CustomerId = p.CustomerId,
                        Payments = new List<Payment>
                        {
                            new Payment
                            {
                                Id = p.PaymentId,
                                PaymentMethod = new PaymentMethod
                                {
                                    Id = p.PaymentId,
                                    PaymentProviderName = GetRandom.String(12),
                                    Type = GetRandom.Enumeration<PaymentMethodType>(),
                                    DisplayName = new LocalizedString
                                    {
                                        { p.CultureInfo.Name, GetRandom.String(12) }
                                    },
                                    Enabled = true,
                                    Default = GetRandom.Boolean(),
                                    PropertyBag = new PropertyBag
                                    {
                                        //TODO: Change this to reflect real return from OV.
                                        { "HostedCardTokenizationUrl", string.Format("https://{0}/?id={1}&css={2}", GetRandom.WwwUrl(), GetRandom.String(16), GetRandom.String(255)) }
                                    }
                                },
                                PaymentStatus = PaymentStatus.New
                            }
                        }
                    };

                    return Task.FromResult(cart);
                }).Verifiable("The repository was not called to initialize the Payment.");

            var vmMapperMock = _container.GetMock<IViewModelMapper>();
            vmMapperMock.Setup(
                vmm => vmm.MapTo<ActivePaymentViewModel>(It.IsNotNull<Payment>(), It.IsNotNull<CultureInfo>()))
                .Returns((Payment p, CultureInfo ci) =>
                {
                    var vm = new ActivePaymentViewModel
                    {
                        Id = p.Id,
                        PaymentStatus = p.PaymentStatus
                    };

                    return vm;
                }).Verifiable("The ViewModelMapper has not been called to create an ActivePaymentViewModel.");
        }

        [Test]
        public async Task WHEN_param_ok_and_cart_SHOULD_return_valid_viewModel()
        {
            //Arrange
            _container.Use(CartViewModelFactoryMock.MockGetPaymentMethodViewModel());

            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(12),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var paymentProviderFactoryMock = _container.GetMock<IPaymentProviderFactory>();
            paymentProviderFactoryMock.Setup(factory => factory.ResolveProvider(It.IsNotNull<string>()))
                .Returns(new FakePaymentProvider());

            var paymentRepoMock = _container.GetMock<IPaymentRepository>();
            paymentRepoMock.Setup(pr => pr.GetPaymentMethodsAsync(It.IsNotNull<GetPaymentMethodsParam>())).ReturnsAsync(
                new List<PaymentMethod>
                {
                    new PaymentMethod
                    {
                        Id = param.PaymentMethodId,                        
                        PaymentProviderName = param.PaymentProviderName,
                        Enabled = true,
                    }
                }
            );
            paymentRepoMock.Setup(pr => pr.GetCartPaymentsAsync(It.IsNotNull<GetCartPaymentsParam>()))
                .ReturnsAsync(new List<Payment>()
                {
                    new Payment
                    {
                        Id = param.PaymentId,
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
                        PaymentMethod = new PaymentMethod
                        {
                            Id = param.PaymentMethodId,
                            Enabled = true
                        }
                    }
                });

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var vm = await sut.UpdateActivePaymentMethodAsync(param);

            //Assert
            vm.Should().NotBeNull();
            vm.Id.Should().NotBeEmpty();
            vm.PaymentStatus.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdatePaymentMethodAsync(null));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_CartName_is_invalid_SHOULD_throw_ArgumentException(string cartName)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = cartName,
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            Expression<Func<Task<CheckoutPaymentViewModel>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_PaymentProviderName_is_invalid_SHOULD_throw_ArgumentException(string paymentProviderName)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = paymentProviderName,
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            Expression<Func<Task<CheckoutPaymentViewModel>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.PaymentProviderName)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\t ")]
        public void WHEN_param_Scope_is_invalid_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = scope
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            Expression<Func<Task<CheckoutPaymentViewModel>>> expression = () => sut.UpdatePaymentMethodAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_param_CultureInfo_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("cultureinfo");
        }

        [Test]
        public void WHEN_param_CustomerId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = Guid.Empty,
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("customerId");
        }

        [Test]
        public void WHEN_param_PaymentId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = Guid.Empty,
                PaymentMethodId = GetRandom.Guid(),
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentId");
        }

        [Test]
        public void WHEN_param_PaymentMethodId_is_invalid_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new UpdatePaymentMethodParam
            {
                CartName = GetRandom.String(10),
                CultureInfo = CultureInfo.InvariantCulture,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                PaymentMethodId = Guid.Empty,
                PaymentProviderName = GetRandom.String(15),
                Scope = GetRandom.String(10)
            };

            var sut = _container.CreateInstance<PaymentViewService>();

            //Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => sut.UpdatePaymentMethodAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo("param");
            exception.Message.Should().ContainEquivalentOf("paymentMethodId");
        }
    }
}
