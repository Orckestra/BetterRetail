using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    interface ITestCategoryViewModelMetadata : IExtensionOf<TestCategoryViewModel>
    {
        string CustomName { get; set; }
    }
}
