using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Services.Breadcrumb
{
    public interface IBreadcrumbViewService
    {
        /// <summary>
        /// Construct a BreadcrumViewModel from the current page ID.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        BreadcrumbViewModel CreateBreadcrumbViewModel(GetBreadcrumbParam param);
    }
}
