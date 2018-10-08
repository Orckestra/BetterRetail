namespace Orckestra.Composer.Search.Helpers
{
    public sealed class SearchSortBy
    {
        /// <summary>
        /// Sorl field name to sort by
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Sorting direction (asc, desc)
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Category where to find the localized display name
        /// </summary>
        public string LocalizationCategory { get; set; }

        /// <summary>
        /// Key of the localized display name
        /// </summary>
        public string LocalizationKey      { get; set; }

        /// <summary>
        /// Key of the localized display name
        /// </summary>
        public SearchType? SearchType { get; set; }
    }
}
