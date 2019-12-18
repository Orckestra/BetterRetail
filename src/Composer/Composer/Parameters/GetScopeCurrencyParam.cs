using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetScopeCurrencyParam
    {
        public string Scope { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }
}
