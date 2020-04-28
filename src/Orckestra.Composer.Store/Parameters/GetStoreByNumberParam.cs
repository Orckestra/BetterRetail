using System.Globalization;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreByNumberParam
    {
        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string StoreNumber { get; set; }

        public string BaseUrl { get; set; }

        public bool IncludeAddresses { get; set; } = true;

        public bool IncludeSchedules { get; set; } = true;

        public Coordinate SearchPoint { get; set; }
    }
}
