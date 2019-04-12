using System.Collections.Generic;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Factory
{
    public interface ILineItemViewModelFactory
    {
        /// <summary>
        /// Gets a list of LineItemDetailViewModel from a list of overture LineItem objects.
        /// </summary>
        IEnumerable<LineItemDetailViewModel> CreateViewModel(CreateListOfLineItemDetailViewModelParam param);

        /// <summary>
        /// Gets the KeyVariant attributes from a line item.
        /// </summary>
        IEnumerable<KeyVariantAttributes> GetKeyVariantAttributes(GetKeyVariantAttributesParam param);
    }
}
