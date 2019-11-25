using Orckestra.Composer.Parameters;
using System;
using System.Globalization;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class BuildSearchQueryUrlParam
    {
        public Guid PageId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public SearchCriteria Criteria { get; set; }
    }
}
