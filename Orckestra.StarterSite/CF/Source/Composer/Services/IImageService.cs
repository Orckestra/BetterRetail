using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public interface IImageService
    {
        Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems);

        Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list);
    }
}
