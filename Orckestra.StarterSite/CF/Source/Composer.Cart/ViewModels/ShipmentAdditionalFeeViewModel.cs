using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ShipmentAdditionalFeeViewModel : BaseViewModel
    {
        /// <summary>
        /// The unique identifier of the additional fee
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the additional fee
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display name of the additional fee
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description of the additional fee
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The amount of the additional fee
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// If the additional fee is taxable
        /// </summary>
        public bool Taxable { get; set; }
    }
}
