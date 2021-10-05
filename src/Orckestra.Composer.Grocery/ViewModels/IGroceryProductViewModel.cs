using System.Collections.Generic;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryProductViewModel : IExtensionOf<ProductViewModel>
    {
        /// <summary>
        /// Gets or sets the product Measure
        /// </summary>
        string BaseProductMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product Size
        /// </summary>
        decimal BaseProductSize { get; set; }

        /// <summary>
        /// Gets or sets the product unit quantity
        /// </summary>
        int ProductUnitQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product unit measure
        /// </summary>
        string ProductUnitMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product unit size
        /// </summary>
        decimal ProductUnitSize { get; set; }

        /// <summary>
        /// Gets or sets the product Converted Volume Measurements
        /// </summary>
        decimal ConvertedVolumeMeasurement { get; set; }
    }
}
