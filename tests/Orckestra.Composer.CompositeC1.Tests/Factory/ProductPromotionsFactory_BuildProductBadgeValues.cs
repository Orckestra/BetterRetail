using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.CompositeC1.Factory;


namespace Orckestra.Composer.CompositeC1.Tests.Factory
{
    [TestFixture]
    public class ProductPromotionsFactory_BuildProductBadgeValues
    {
        public AutoMocker Container { get; set; }

        // mocks
        private Mock<IProductTileConfigurationContext> _productTileConfigurationContext;

      
        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
            // IComposerContext
            _productTileConfigurationContext = new Mock<IProductTileConfigurationContext>();
            Container.Use(_productTileConfigurationContext);
        }

        [Test]
        public void WHEN_keys_are_null_SHOULD_return_null()
        {
            // arrange
            var factory = Container.CreateInstance<ProductPromotionsFactory>();
            var displayNames = "DN1,DN2";

            // act
            var badgeValues = factory.BuildProductBadgeValues(null, displayNames);

            // assert
            badgeValues.Should().BeNull();
        }

        [Test]
        public void WHEN_correct_values_SHOULD_return_badge_values()
        {
            // arrange
            var factory = Container.CreateInstance<ProductPromotionsFactory>();
            var keys = "K1|K2";
            var displayNames = "DN1,DN2";

            // act
            var badgeValues = factory.BuildProductBadgeValues(keys, displayNames);

            // assert
            badgeValues.Should().NotBeNull();
            badgeValues.Count.Should().Be(2);
            badgeValues["K1"].Should().Be("DN1");
            badgeValues["K2"].Should().Be("DN2");
        }

        [Test]
        public void WHEN_displayName_is_missing_SHOULD_return_key_value()
        {
            // arrange
            var factory = Container.CreateInstance<ProductPromotionsFactory>();
            var keys = "K1|K2";
            var displayNames = "DN1";

            // act
            var badgeValues = factory.BuildProductBadgeValues(keys, displayNames);

            // assert
            badgeValues.Should().NotBeNull();
            badgeValues.Count.Should().Be(2);
            badgeValues["K1"].Should().Be("DN1");
            badgeValues["K2"].Should().Be("K2");
        }

        [Test]
        public void WHEN_displayNames_are_empty_SHOULD_return_key_values()
        {
            // arrange
            var factory = Container.CreateInstance<ProductPromotionsFactory>();
            var keys = "K1|K2";
            var displayNames = "";

            // act
            var badgeValues = factory.BuildProductBadgeValues(keys, displayNames);

            // assert
            badgeValues.Should().NotBeNull();
            badgeValues.Count.Should().Be(2);
            badgeValues["K1"].Should().Be("K1");
            badgeValues["K2"].Should().Be("K2");
        }
    };
}