using System;
using System.Linq.Expressions;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Overture;
using static Orckestra.Composer.Utils.ExpressionUtility;

namespace Orckestra.Composer.Cart.Tests.Factory.Order
{
    public class ShippingTrackingProviderFactoryResolverProvider
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            _container.Use((IShippingTrackingProviderRegistry)new ShippingTrackingProviderRegistry());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\r\n     ")]
        public void WHEN_name_is_invalid_SHOULD_throw_ArgumentException(string name)
        {
            //Arrange
            var sut = _container.CreateInstance<ShippingTrackingProviderFactory>();

            //Act
            Expression<Func<IShippingTrackingProvider>> expression = () => sut.ResolveProvider(name);
            var exception = Assert.Throws<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [Test]
        public void WHEN_registry_is_empty_SHOULD_throw_InvalidOperationException()
        {
            //Arrange
            var sut = _container.CreateInstance<ShippingTrackingProviderFactory>();

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => sut.ResolveProvider(GetRandom.String(5)));

            //Assert
            exception.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_unknow_provider_name_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var registry = _container.Get<IShippingTrackingProviderRegistry>();
            registry.RegisterProvider<FakeShippingTrackingProvider>(GetRandom.String(15));

            var sut = _container.CreateInstance<ShippingTrackingProviderFactory>();

            //Act
            ArgumentException exception = null;
            exception = Assert.Throws<ArgumentException>(() => sut.ResolveProvider(GetRandom.String(20)));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().BeEquivalentTo("name");
        }

        [Test]
        public void WHEN_known_provider_name_SHOULD_retrieve_instance()
        {
            //Arrange
            var registry = _container.Get<IShippingTrackingProviderRegistry>();
            var providerName = GetRandom.String(15);
            registry.RegisterProvider(providerName, typeof(FakeShippingTrackingProvider));

            var mockResolver = _container.GetMock<IDependencyResolver>();
            mockResolver.Setup(dr => dr.Resolve(It.IsNotNull<Type>()))
                .Returns(new FakeShippingTrackingProvider());

            var sut = _container.CreateInstance<ShippingTrackingProviderFactory>();

            //Act
            IShippingTrackingProvider provider = sut.ResolveProvider(providerName);

            //Assert
            provider.Should().NotBeNull();
            provider.ProviderName.Should().Be(providerName);
        }


    }
}
