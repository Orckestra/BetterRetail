using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StorePageViewModel: BaseViewModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public int Page { get; set; }

        public bool IsCurrentPage { get; set; }
    }
}
