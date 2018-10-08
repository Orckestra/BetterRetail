using System.Globalization;

namespace Orckestra.Composer.Product.Parameters
{
    /// <summary>
    /// Parameter object to retrieve the product definition object
    /// </summary>
    public class GetProductDefinitionParam
    {
        public string Name { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
