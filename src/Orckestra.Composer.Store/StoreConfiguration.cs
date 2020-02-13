
namespace Orckestra.Composer.Store
{
    public static class StoreConfiguration
    {
        /// <summary>
        ///     Gets or sets the maximum items per page.
        /// </summary>
        /// <value>
        ///     The maximum items per page.
        /// </value>
        public static int StoreLocatorMaxItemsPerPage { get; set; } = 9;

        public static int DirectoryListMaxItemsPerPage { get; set; } = 30;

        public static int InventoryListMaxItemsPerPage { get; set; } = 4;
    }
}
