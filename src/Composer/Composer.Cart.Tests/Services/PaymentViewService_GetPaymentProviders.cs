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
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Overture.ServiceModel.Providers;
using System;
using Orckestra.Overture.Providers;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class PaymentViewServiceGetPaymentProviders
    {
        private AutoMocker _container;
        private Mock<IPaymentProviderFactory> _paymentProviderMoq;
        private Mock<IPaymentRepository> _paymentRepoMoq;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var mapper = ViewModelMapperFactory.CreateFake(typeof(PaymentProviderViewModel).Assembly);

            _container.Use(mapper);

            _paymentProviderMoq = new Mock<IPaymentProviderFactory>();
            _container.Use(_paymentProviderMoq.Object);

            _paymentRepoMoq = new Mock<IPaymentRepository>();
            _container.Use(_paymentRepoMoq.Object);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = CultureInfo.InvariantCulture,
            };
            var service = _container.CreateInstance<PaymentViewService>();

            var (provider, paymentProviderInfo) = ConfigureDefaultProviders(param);

            // act
            var result = await service.GetPaymentProvidersAsync(param);

            // assert
            result.Should().HaveCount(1);
        }

        [Test]
        public async Task WHEN_Provider_Different_SHOULD_Filter_out_value()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = CultureInfo.InvariantCulture,
            };
            var service = _container.CreateInstance<PaymentViewService>();
            var (provider, paymentProviderInfo) = ConfigureDefaultProviders(param);
            provider.ImplementationTypeName = GetRandom.String(16);

            // act
            var result = await service.GetPaymentProvidersAsync(param);

            // assert
            result.Should().HaveCount(0);
        }

        [Test]
        public async Task WHEN_Provider_Not_Active_SHOULD_Filter_out_value()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = CultureInfo.InvariantCulture,
            };
            var service = _container.CreateInstance<PaymentViewService>();
            var (provider, paymentProviderInfo) = ConfigureDefaultProviders(param);
            provider.IsActive = false;

            // act
            var result = await service.GetPaymentProvidersAsync(param);

            // assert
            result.Should().HaveCount(0);
        }


        [Test]
        public async Task WHEN_Culture_not_supported_SHOULD_Filter_out_value()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
            };
            var service = _container.CreateInstance<PaymentViewService>();

            var (provider, paymentProviderInfo) = ConfigureDefaultProviders(param);
            paymentProviderInfo.SupportedCultureIds = "ua-UK";

            // act
            var result = await service.GetPaymentProvidersAsync(param);

            // assert
            result.Should().HaveCount(0);
        }

        [Test]
        public void When_Passing_Empty_Param_SHOULD_throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<PaymentViewService>();
            ConfigureEmptyProviders();

            // act & assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.GetPaymentProvidersAsync(null));
        }

        [Test]
        public void When_Passing_Empty_Scope_SHOULD_throw_ArgumentException()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = null,
                CultureInfo = CultureInfo.InvariantCulture,
            };
            var service = _container.CreateInstance<PaymentViewService>();
            ConfigureEmptyProviders();

            // act & assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetPaymentProvidersAsync(param));
        }

        [Test]
        public void When_Passing_Empty_CultureInfo_SHOULD_throw_ArgumentException()
        {
            // arrange
            var param = new GetPaymentProvidersParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = null,
            };
            var service = _container.CreateInstance<PaymentViewService>();
            ConfigureEmptyProviders();

            // act & assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetPaymentProvidersAsync(param));
        }

        private void ConfigureEmptyProviders()
        {
            _paymentProviderMoq.Setup(q => q.ResolveAllProviders()).Returns(new List<IPaymentProvider>());
            _paymentRepoMoq.Setup(q => q.GetPaymentProviders(It.IsAny<string>())).ReturnsAsync(new List<PaymentProvider>());
        }

        private (Provider provider, PaymentProvider paymentProvider) ConfigureDefaultProviders(GetPaymentProvidersParam param)
        {
            var localPaymentProvider = new FakePaymentProvider();

            _paymentProviderMoq
                .Setup(q => q.ResolveAllProviders())
                .Returns(new List<IPaymentProvider> {localPaymentProvider});

            var id = Guid.NewGuid();

            var provider = new Provider
            {
                Id = id,
                ImplementationTypeName = localPaymentProvider.ProviderType,
                IsActive = true,
            };

            var paymentProvider = new PaymentProvider
            {
                Id = id,
                SupportedCultureIds = param.CultureInfo.Name,
            };

            _paymentRepoMoq
                .Setup(q => q.GetProviders(It.IsAny<string>(), ProviderType.Payment))
                .ReturnsAsync(new List<Provider>
                {
                    provider
                });


            _paymentRepoMoq
                .Setup(q => q.GetPaymentProviders(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentProvider>
                {
                    paymentProvider,
                });

            return (provider, paymentProvider);
        }
    }
}
