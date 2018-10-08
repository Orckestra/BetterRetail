using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Search.Services
{
    public interface ISearchBreadcrumbViewService
    {
        /// <summary>
        /// Creates a <see cref="BreadcrumbViewModel"/> for given search keywords.
        /// </summary>
        /// <param name="param">Parameters to generate the ViewModel.</param>
        /// <returns></returns>
        BreadcrumbViewModel CreateBreadcrumbViewModel(GetSearchBreadcrumbParam param);
    }
}
