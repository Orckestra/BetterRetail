using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetCategoriesPathParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string CategoryId { get; set; }
    }
}
