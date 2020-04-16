using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class LineItemIdsViewModel : BaseViewModel
    {
        /// <summary>
        /// LineItem Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Product Variant Id
        /// </summary>
        public string VariantId { get; set; }
    }
}
