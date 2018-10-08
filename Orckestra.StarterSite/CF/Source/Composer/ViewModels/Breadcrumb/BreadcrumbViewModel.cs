using System.Collections.Generic;

namespace Orckestra.Composer.ViewModels.Breadcrumb
{
    public sealed class BreadcrumbViewModel : BaseViewModel
    {
        public string ActivePageName { get; set; }
        public List<BreadcrumbItemViewModel> Items { get; set; }

        public BreadcrumbViewModel()
        {
            Items = new List<BreadcrumbItemViewModel>();
        }
    }
}
