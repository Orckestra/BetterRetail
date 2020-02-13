
using System.Globalization;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Parameters
{
    public class CreateStoreViewModelParam
    {
        public Overture.ServiceModel.Customers.Stores.Store Store { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public string BaseUrl { get; set; }

        public Coordinate SearchPoint { get; set; }
    }
}
