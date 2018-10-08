using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Cart.Services
{
    public interface ICheckoutBreadcrumbViewService
    {
        /// <summary>
        /// Creates a <see cref="BreadcrumbViewModel"/> for given search keywords.
        /// </summary>
        /// <param name="param">Parameters to generate the ViewModel.</param>
        /// <returns></returns>
        BreadcrumbViewModel CreateBreadcrumbViewModel(GetCheckoutBreadcrumbParam param);
    }
}
