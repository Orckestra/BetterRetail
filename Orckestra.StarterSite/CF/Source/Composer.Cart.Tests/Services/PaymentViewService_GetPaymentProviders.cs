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
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Overture.ServiceModel.Providers;
using System;

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
            var paymentProvider = new FakePaymentProvider();

            _paymentProviderMoq
                .Setup(q => q.ResolveAllProviders())
                .Returns(new List<IPaymentProvider> { paymentProvider });

            _paymentRepoMoq
                .Setup(q => q.GetPaymentProviders(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentProviderInfo>
                    {
                        new PaymentProviderInfo
                        {
                            ImplementationTypeName = paymentProvider.ProviderType,
                            SupportedCultureIds = param.CultureInfo.Name,
                        }
                    });

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
            var paymentProvider = new FakePaymentProvider();

            _paymentProviderMoq
                .Setup(q => q.ResolveAllProviders())
                .Returns(new List<IPaymentProvider> { paymentProvider });

            _paymentRepoMoq
                .Setup(q => q.GetPaymentProviders(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentProviderInfo>
                    {
                        new PaymentProviderInfo
                        {
                            ImplementationTypeName = GetRandom.String(16),
                            SupportedCultureIds = param.CultureInfo.Name,
                        }
                    });

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
            var paymentProvider = new FakePaymentProvider();

            _paymentProviderMoq
                .Setup(q => q.ResolveAllProviders())
                .Returns(new List<IPaymentProvider> { paymentProvider });

            _paymentRepoMoq
                .Setup(q => q.GetPaymentProviders(It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentProviderInfo>
                    {
                        new PaymentProviderInfo
                        {
                            ImplementationTypeName = paymentProvider.ProviderType,
                            SupportedCultureIds = "ua-UK",
                        }
                    });

            // act
            var result = await service.GetPaymentProvidersAsync(param);

            // assert
            result.Should().HaveCount(0);
        }

        [Test]
        public void When_Passing_Empty_Param_SHOULD_throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<PaymentViewService>();
            _paymentProviderMoq.Setup(q => q.ResolveAllProviders()).Returns(new List<IPaymentProvider>());
            _paymentRepoMoq.Setup(q => q.GetPaymentProviders(It.IsAny<string>())).ReturnsAsync(new List<PaymentProviderInfo>());

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
            _paymentProviderMoq.Setup(q => q.ResolveAllProviders()).Returns(new List<IPaymentProvider>());
            _paymentRepoMoq.Setup(q => q.GetPaymentProviders(It.IsAny<string>())).ReturnsAsync(new List<PaymentProviderInfo>());

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
            _paymentProviderMoq.Setup(q => q.ResolveAllProviders()).Returns(new List<IPaymentProvider>());
            _paymentRepoMoq.Setup(q => q.GetPaymentProviders(It.IsAny<string>())).ReturnsAsync(new List<PaymentProviderInfo>());

            // act & assert
            Assert.ThrowsAsync<ArgumentException>(() => service.GetPaymentProvidersAsync(param));
        }

    }
}
