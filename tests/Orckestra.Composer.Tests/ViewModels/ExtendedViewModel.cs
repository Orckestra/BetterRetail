using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    public interface IExtensionViewModel : IExtensionOf<ExtendedViewModel>
    {
        int CustomProperty { get; set; }
        bool CustomBool { get; set; }
    }

    public sealed class ExtendedViewModel : BaseViewModel
    {
        public string BaseProperty { get; set; }
    }
}