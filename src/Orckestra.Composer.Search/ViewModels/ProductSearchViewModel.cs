using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public class ProductSearchViewModel : BaseProductViewModel
    {
        /// <summary>
        /// Gets or sets the brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the full display name.
        /// </summary>
        [MapTo("DisplayName")]
        public string FullDisplayName { get; set; }
        public double? Price { get; set; }
        public double? ListPrice { get; set; }
        public bool IsWeightedProduct { get; set; }
        /// <summary>
        /// Gets or sets the search term
        /// </summary>
        public string SearchTerm { get; set; }
    }
}
