using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Overture;

namespace Orckestra.Composer.Search.Tests.Repository
{
    [TestFixture]
    public class SearchRepositoryCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var overtureClient = new Mock<IOvertureClient>();
            var productFactory = new Mock<IProductRequestFactory>();
            var facetPredicateFactory = new Mock<IFacetPredicateFactory>();

            // Act
            Action action = () => new SearchRepository(overtureClient.Object, productFactory.Object, facetPredicateFactory.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var productFactory = new Mock<IProductRequestFactory>();
            var facetPredicateFactory = new Mock<IFacetPredicateFactory>();

            // Act
            Action action = () => new SearchRepository(null, productFactory.Object, facetPredicateFactory.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_ProductRequestFactory_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var overtureClient = new Mock<IOvertureClient>();
            var facetPredicateFactory = new Mock<IFacetPredicateFactory>();

            // Act
            Action action = () => new SearchRepository(overtureClient.Object, null, facetPredicateFactory.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_FacetPredicateFactory_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var overtureClient = new Mock<IOvertureClient>();
            var productFactory = new Mock<IProductRequestFactory>();

            // Act
            Action action = () => new SearchRepository(overtureClient.Object, productFactory.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
