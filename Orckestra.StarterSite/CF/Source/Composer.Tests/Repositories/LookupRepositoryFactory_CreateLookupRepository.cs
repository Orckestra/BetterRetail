using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    public class LookupRepositoryFactory_CreateLookupRepository : BaseTest
    {
        private ILookupRepositoryFactory _factory; 
        [SetUp]
        public void Setup()
        {
            
        _factory = Container.CreateInstance<LookupRepositoryFactory>();
        }
        [Test]
        public void WHEN_LookupType_Is_Product_SHOULD_Return_ProductLookupRepository()
        {
            var repo = _factory.CreateLookupRepository(LookupType.Product);

            repo.Should().BeAssignableTo<ProductLookupRepository>();

        }
        [Test]
        public void WHEN_LookupType_Is_Marketing_SHOULD_Return_MarketingLookupRepository()
        {
            var repo = _factory.CreateLookupRepository(LookupType.Marketing);

            repo.Should().BeAssignableTo<MarketingLookupRepository>();

        }

        [Test]
        public void WHEN_LookupType_Is_Order_SHOULD_Return_OrderLookupRepository()
        {
            var repo = _factory.CreateLookupRepository(LookupType.Order);

            repo.Should().BeAssignableTo<OrderLookupRepository>();

        }
    }
}
