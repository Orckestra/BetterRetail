using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    public interface INonExtensionViewModel
    {
        string CustomProperty { get; set; }
    }

    public interface IViewModelExtensionWithMissingPropertyGetter : IExtensionOf<ExtendedViewModel>
    {
        int WriteOnlyProperty { set; }
    }

    public interface IViewModelExtensionWithMissingPropertySetter : IExtensionOf<ExtendedViewModel>
    {
        int ReadOnlyProperty { get; }
    }

    public interface IViewModelExtensionWithMethod : IExtensionOf<ExtendedViewModel>
    {
        void Method();
    }
}