using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;

namespace Orckestra.Composer.Search.Tests.Repository
{
    [TestFixture]
    public class SearchRepositoryCtor
    {
        private IComposerOvertureClient _overtureClient;
        private IProductRequestFactory _productFactory;
        private IFacetPredicateFactory _facetPredicateFactory;
        private IFacetConfigurationContext _facetConfigurationContext;

        [SetUp]
        public void SetUp()
        {
            _overtureClient = new Mock<IComposerOvertureClient>().Object;
            _productFactory = new Mock<IProductRequestFactory>().Object;
            _facetPredicateFactory = new Mock<IFacetPredicateFactory>().Object;
            _facetConfigurationContext = new Mock<IFacetConfigurationContext>().Object;
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Act
            Action action = () => new SearchRepository(_overtureClient, _productFactory, _facetPredicateFactory, _facetConfigurationContext);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_ArgumentNullException()
        {
            // Act
            Action action = () => new SearchRepository(null, _productFactory, _facetPredicateFactory, _facetConfigurationContext);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_ProductRequestFactory_SHOULD_Throw_ArgumentNullException()
        {
            // Act
            Action action = () => new SearchRepository(_overtureClient, null, _facetPredicateFactory, _facetConfigurationContext);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_FacetPredicateFactory_SHOULD_Throw_ArgumentNullException()
        {
            // Act
            Action action = () => new SearchRepository(_overtureClient, _productFactory, null, _facetConfigurationContext);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_FacetConfigurationContext_SHOULD_Throw_ArgumentNullException()
        {
            // Act
            Action action = () => new SearchRepository(_overtureClient, _productFactory, _facetPredicateFactory, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

    }
}
