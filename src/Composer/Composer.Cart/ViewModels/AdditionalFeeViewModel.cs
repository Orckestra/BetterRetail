using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class AdditionalFeeViewModel : BaseViewModel
    {
        /// <summary>
        /// The unique identifier of the additional fee.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the additional fee.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display name of the additional fee.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description of the additional fee.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The amount of the additional fee.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Amount { get; set; }

        /// <summary>
        /// Whether additional fee is taxable or not.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// The calculation of the additional fee. This property value is a enum of AdditionalFeeCalculationRule
        /// </summary>
        [EnumLocalization(LocalizationCategory = "ShoppingCart", AllowEmptyValue = true)]
        public string CalculationRule { get; set; }

        /// <summary>
        /// TotalAmount = Amount * Quantity if CalculationRule = PerItem
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
