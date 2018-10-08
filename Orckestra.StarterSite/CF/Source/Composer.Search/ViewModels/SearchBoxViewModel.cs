using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    //Used in Sitecore and C1
    public sealed class SearchBoxViewModel : BaseViewModel
    {
        public string Keywords { get; set; }
        public string ActionTarget { get; set; }
    }
}
