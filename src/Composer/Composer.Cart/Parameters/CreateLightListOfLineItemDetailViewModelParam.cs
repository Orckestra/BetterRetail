using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateLightListOfLineItemDetailViewModelParam
    {
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }
        public List<LineItem> LineItems { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public ProductImageInfo ImageInfo { get; set; }
        public string BaseUrl { get; set; }
    }
}