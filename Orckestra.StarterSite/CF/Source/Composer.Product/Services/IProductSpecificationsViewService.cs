using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Services
{
    /// <summary>
    /// Service for dealing with Product Specifications.
    /// </summary>
    public interface IProductSpecificationsViewService
    {
        /// <summary>
        /// Gets a <see cref="SpecificationsViewModel" /> for a given product.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Instance of <see cref="SpecificationsViewModel" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        /// <exception cref="System.ArgumentException">param</exception>
        Task<SpecificationsViewModel> GetProductSpecificationsViewModelAsync(GetProductSpecificationsParam param);

        /// <summary>
        /// Gets a <see cref="SpecificationsViewModel" /> for a given product. The specifications group collection is empty.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Instance of <see cref="SpecificationsViewModel" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        /// <exception cref="System.ArgumentException">param</exception>
        SpecificationsViewModel GetEmptySpecificationsViewModel(GetProductSpecificationsParam param);
    }
}
