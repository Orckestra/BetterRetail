using System;
using System.Collections.Generic;
using System.Globalization;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture(Category = "ProductRepository")]
    public class ProductRepositoryCalculatePriceAsync
    {
        private static readonly List<string> ProductsList = new List<string> { "abc123", "def456" };

        private CultureInfo _englishCultureInfo;

        [SetUp]
        public void SetUp()
        {
            _englishCultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        }

        private object[] GetProductParams()
        {
            return new object[] { new object[] { ProductsList, null } };
        }

        [TestCase(null, "scope")]
        [Test, TestCaseSource("GetProductParams")]
        public void When_Any_Argument_Null_Or_Whitespace_SHOULD_Throw_Null_Argument_Exception(List<string> productIds, string scope )
        {
            //Arrange
            var container = new AutoMocker();
            var productRepository = container.CreateInstance<ProductRepository>();

            //Act & Assert
            Assert.Throws<ArgumentNullException>(async () =>
            {
                await productRepository.CalculatePricesAsync(productIds, scope);
            });
        }
    }
}
