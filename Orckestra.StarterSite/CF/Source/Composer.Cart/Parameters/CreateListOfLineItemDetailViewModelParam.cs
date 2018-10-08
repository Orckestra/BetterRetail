using System.Collections.Generic;
using System.Globalization;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateListOfLineItemDetailViewModelParam
    {
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }

        public IEnumerable<LineItem> LineItems { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public ProductImageInfo ImageInfo { get; set; }

        public string BaseUrl { get; set; }
    }
}
