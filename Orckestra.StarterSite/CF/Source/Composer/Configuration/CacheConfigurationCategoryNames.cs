namespace Orckestra.Composer.Configuration
{
    /// <summary>
    /// This class defines a list of constant names that will be used when getting/setting information in cache for different systems
    /// </summary>
    public static class CacheConfigurationCategoryNames
    {
        /// <summary>
        /// Cache name where Product information will be stored
        /// </summary>
        public static string Product = "Product";

        /// <summary>
        /// Cache name where ProductDefinition information will be stored
        /// </summary>
        public static string ProductDefinition = "ProductDefinition";

        /// <summary>
        /// Cache name where Lookups for various objects types will be stored
        /// </summary>
        public static string Lookup = "Lookup";

        /// <summary>
        /// Cache name where Country information will be stored
        /// </summary>
        public static string Country = "Country";

        public static string Regions = "Regions";

        public static string ProductLookup = "ProductLookup";

        public static string Cart = "Cart";
        public static string CartPayment = "CartPayment";

        public static string Category = "Category";

        public static string LocalizationTree = "LocalizationTree";

        public static string HandlebarsView = "HandlebarsView";

        public static string FulfillmentMethod = "FulfillmentMethod";

        public static string PaymentMethod = "PaymentMethod";

        public static string CategoryUrls = "CategoryUrls";

        /// <summary>
        /// Cache name where Customers information will be stored
        /// </summary>
        public static string Customer = "Customer";

        /// <summary>
        /// Cache name where Addresses information will be stored
        /// </summary>
        public static string Address = "Address";

        public const string CheckoutStepUrls = "CheckoutStepUrls";

        /// <summary>
        /// Cache where the Product Settings will be stored
        /// </summary>
        public static string ProductSettings = "ProductSettings";

         /// <summary>
        /// Cache where the Product Media Settings will be stored
        /// </summary>
        public static string ProductMediaSettings = "ProductMediaSettings";

        public static string FulfillmentLocationsByScope = "FulfillmentLocationsByScope";

        /// <summary>
        /// Cache used to store scopes.
        /// </summary>
        public static string Scopes = "Scopes";

        public static string Store = "Store";

        public static string StoreSchedule = "StoreSchedule";

        public static string StoreInventoryItems = "StoreInventoryItems";

        public static string RecurringOrderPrograms = "RecurringOrderPrograms";

        /// <summary>
        /// Cache used for the list of fulfillementMEthods for a scope
        /// </summary>
        public static string FulfillmentMethodsByScope = "FulfillmentMethodsByScope";

        public static string PaymentProviders = "PaymentProviders";
    }
}
