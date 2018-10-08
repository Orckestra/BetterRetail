using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    public interface ILineItemService
    {
        Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems);

        List<LineItem> GetInvalidLineItems(ProcessedCart cart);
    }
}
