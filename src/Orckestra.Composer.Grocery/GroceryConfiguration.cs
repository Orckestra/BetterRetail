namespace Orckestra.Composer.Grocery
{
    public static class GroceryConfiguration
    {
        public static readonly string FulfillmentTimeSlot = nameof(FulfillmentTimeSlot);

        public static int MaxOrderedProductsItems { get; set; } = 3000;
    }
}
