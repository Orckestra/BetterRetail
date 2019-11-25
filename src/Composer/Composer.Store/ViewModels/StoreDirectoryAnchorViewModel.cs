
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreDirectoryAnchorViewModel: BaseViewModel
    {
        public string DisplayName { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
    }
}
