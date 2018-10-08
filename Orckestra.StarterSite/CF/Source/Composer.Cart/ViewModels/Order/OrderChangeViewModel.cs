using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderChangeViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the identifier for an history.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The date this item was changed
        /// </summary>
        [MapTo("CreatedDate")]
        [Formatting("General", "ShortDateFormat")]
        public string Date { get; set; }

        /// <summary>
        /// the category of this history item
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The old value related to this history change.
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// the new value related to this history change
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// the comment associated to this history change
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// the reason selected for this history change
        /// </summary>
        public string Reason { get; set; }
    }
}
