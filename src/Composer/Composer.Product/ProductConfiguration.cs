using System.Collections.Generic;

namespace Orckestra.Composer.Product
{
    /// <summary>
    /// Product-specific configurations.
    /// </summary>
    public static class ProductConfiguration
    {
        public static string ThumbnailImageSize = "S";
        public static string ImageSize = "L";
        public static string ProductSummaryImageSize = "L";
        public static string ProductZoomImageSize = "XL";
        public static string LookupCacheCategory = "Lookup";

        public static bool IsQuantityDisplayed = true;
        public static int MinQuantity = 1;
        public static int MaxQuantity = 99;

    }

    public static class SpecificationConfiguration
    {
        public static int MaxRelatedProducts = 4;

        public static List<string> ExcludedSpecificationPropertyGroups = new List<string> { "Default" };
        public static List<string> ExcludedSpecificationProperty = new List<string> { "ProductDefinition" };
        public static string MultiValueLookupSeparator = ",";
    }
}
