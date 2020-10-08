using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Configuration
{
    /// <summary>
    /// Product-quantity configurations.
    /// </summary>
    public static class QuantityConfiguration
    {
        public static bool IsQuantityDisplayed = true;
        public static int MinQuantity = 1;
        public static int MaxQuantity = 99;

        public static ProductQuantityViewModel GetProductQuantity()
        {
            return IsQuantityDisplayed && MaxQuantity >= MinQuantity ? new ProductQuantityViewModel { Min = MinQuantity, Max = MaxQuantity, Value = 1 } : null;
        }
    }
}
