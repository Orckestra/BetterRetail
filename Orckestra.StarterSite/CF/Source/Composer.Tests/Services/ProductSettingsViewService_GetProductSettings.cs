using System;
using System.Threading.Tasks;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.Services
{
    [TestFixture]
    public class ProductSettingsViewService_GetProductSettings : BaseTest
    {
        [Test]
        public void When_Passing_Null_Scope_Throw_ArgumentNullException()
        {
            var productSettingsViewService = Container.CreateInstance<ProductSettingsViewService>();

            Assert.ThrowsAsync<ArgumentException>(() => productSettingsViewService.GetProductSettings(null, TestingExtensions.GetRandomCulture()));
        }

        [Test]
        public void When_Passing_Null_Culture_Throw_ArgumentNullException()
        {
            var productSettingsViewService = Container.CreateInstance<ProductSettingsViewService>();

            Assert.ThrowsAsync<ArgumentNullException>(() => productSettingsViewService.GetProductSettings(GetRandom.String(32), null));
        }

        [Test]
        public async Task When_Passing_Valid_Scope_Return_ProductSettingsViewModel()
        {
            var productSettingsViewService = Container.GetMock<IProductSettingsViewService>();

            productSettingsViewService
                .Setup(p => p.GetProductSettings(It.IsAny<string>(), It.IsAny<CultureInfo>()))
                .ReturnsAsync(new ProductSettingsViewModel { IsInventoryEnabled = true });

            var productSettings = await productSettingsViewService.Object.GetProductSettings(GetRandom.String(32), TestingExtensions.GetRandomCulture());

            Assert.NotNull(productSettings);
            Assert.AreEqual(productSettings.IsInventoryEnabled, true);
        }
    }
}
