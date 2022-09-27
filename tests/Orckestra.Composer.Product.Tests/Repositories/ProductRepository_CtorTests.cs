using System;
using System.Reflection;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture(Category = "ProductRepository")]
    public class ProductRepositoryCtorTests
    {
        [Test]
        public void WhenNullOvertureClient_ThrowsNullArgumentException()
        {
            //Arrange
            var container = new AutoMocker();
            container.Use((IComposerOvertureClient) null);

            //Act & Assert
            var nullException = Assert.Throws<TargetInvocationException>(() =>
            {
                container.CreateInstance<ProductRepository>();
            });

            nullException.InnerException.Should().BeOfType<ArgumentNullException>();
            nullException.InnerException.Message.Should().Contain("overtureClient");
        }

        [Test]
        public void WhenOkParameters_NoExceptionRaised()
        {
            //Arrange
            var container = new AutoMocker();

            //Act & Assert
            Assert.DoesNotThrow(() =>
            {
                container.CreateInstance<ProductRepository>();
            });
        }
    }
}
