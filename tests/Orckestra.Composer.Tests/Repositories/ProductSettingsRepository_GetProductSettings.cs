using System;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Tests.Repositories
{
    [TestFixture]
    public class ProductSettingsRepository_GetProductSettings : BaseTest
    {
        [Test]
        public void When_Passing_Null_Scope_Throw_ArgumentNullException()
        {
            var productSettingsRepository = Container.CreateInstance<ProductSettingsRepository>();

            Assert.ThrowsAsync<ArgumentException>(() => productSettingsRepository.GetProductSettings(null));
        }

        [Test]
        public async Task When_Passing_Valid_Scope_Return_ProductSettings()
        {
            var productSettingsRepository = Container.GetMock<IProductSettingsRepository>();

            productSettingsRepository
                .Setup(p => p.GetProductSettings(It.IsAny<string>()))
                .ReturnsAsync(new ProductSettings{ IsInventoryEnabled = true });

            var productSettings = await productSettingsRepository.Object.GetProductSettings(GetRandom.String(32));

            Assert.NotNull(productSettings);
            Assert.AreEqual(productSettings.IsInventoryEnabled, true);
        }
    }
}
