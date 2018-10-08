using System.Globalization;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.Parameters
{
    public class GetLookupDisplayNameParam
    {
        public GetLookupDisplayNameParam()
        {
            Delimiter = ", ";
        }
        public LookupType LookupType { get; set; }
        public string LookupName { get; set; }
        public string Value { get; set; }
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Orverture can store multiple lookup values for a single property.
        /// When converting lookup values to 
        /// </summary>
	    public string Delimiter { get; set; }
    }
}
