using System;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreScheduleParam
    {
        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public Guid FulfillmentLocationId { get; set; }
    }
}
