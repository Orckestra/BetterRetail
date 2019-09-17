using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    /// <summary>
    /// Parameter object to get products prices.
    /// </summary>
    public class GetProductsPriceParam
    {
        public List<string> ProductIds { get; set; }
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public GetProductsPriceParam()
        {
            ProductIds = new List<string>();
        }
    }
}
