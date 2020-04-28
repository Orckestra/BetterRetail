using System;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreParam
    {
        public Guid Id { get; set; }

        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public bool IncludeAddresses { get; set; } = true;

        public bool IncludeSchedules { get; set; } = true;

        public bool IncludeOperatingStatus { get; set; } = true;
    }
}
