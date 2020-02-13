using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    /// <summary>
    /// Search pagination
    /// </summary>
    public sealed class StorePaginationViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets properties for the [Previous] page button.
        /// </summary>
        /// <value>
        /// The previous page.
        /// </value>
        public StorePageViewModel PreviousPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the [Next] page button.
        /// </summary>
        /// <value>
        /// The next page.
        /// </value>
        public StorePageViewModel NextPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the paged result set.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        public IEnumerable<StorePageViewModel> Pages { get; set; }
    }
}