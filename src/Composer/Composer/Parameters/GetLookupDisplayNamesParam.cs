using System.Globalization;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.Parameters
{
    public class GetLookupDisplayNamesParam
    {
        public LookupType LookupType { get; set; }
        public string LookupName { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
