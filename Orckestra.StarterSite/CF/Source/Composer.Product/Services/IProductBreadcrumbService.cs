using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Product.Services
{
    public interface IProductBreadcrumbService
    {
        /// <summary>
        /// Creates a <see cref="BreadcrumbViewModel"/> for a given product.
        /// </summary>
        /// <param name="parameters">Parameters to generate the ViewModel.</param>
        /// <returns></returns>
        Task<BreadcrumbViewModel> CreateBreadcrumbAsync(GetProductBreadcrumbParam parameters);
    }
}
