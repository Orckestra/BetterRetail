using System.Globalization;
using System.Reflection;
using Moq;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Mock
{
    public static class FakeViewModelMapper
    {
        internal static IViewModelMapper CreateFake(params Assembly[] assemblies)
        {
            var registry = new ViewModelMetadataRegistry();

            foreach (var assembly in assemblies)
            {
                registry.LoadViewModelMetadataInAssemblyOf(assembly);
            }

            var formatterMock = new Mock<IViewModelPropertyFormatter>();
            formatterMock.Setup(m => m.Format(It.IsAny<object>(), It.IsNotNull<IPropertyMetadata>(), It.IsAny<CultureInfo>()))
                .Returns((object value, IPropertyMetadata meta, CultureInfo culture) => value?.ToString());

            var lookupServiceMock = new Mock<ILookupService>();
            var localizationProviderMock = new Mock<ILocalizationProvider>();

            var mapper = new ViewModelMapper(registry, formatterMock.Object, lookupServiceMock.Object, localizationProviderMock.Object);
            return mapper;
        }
    }
}
