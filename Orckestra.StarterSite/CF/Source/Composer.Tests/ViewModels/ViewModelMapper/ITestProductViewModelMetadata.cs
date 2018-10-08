using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public interface ITestProductViewModelMetadata : IExtensionOf<TestProductViewModel>
    {
        int CustomProperty { get; set; }
        [MapTo("MappedCustomProperty")]
        string MappedViewModelBagProperty { get; set; }
    }
}
