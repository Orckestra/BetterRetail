using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class TaxViewModel : BaseViewModel
    {
        /// <summary>
        /// The code of the tax.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The tax UI-friendly display name.
        /// </summary>
        public string DisplayName { get; set; }

        [Formatting("General", "PercentageFormat")]
        public string Percentage { get; set; }

        /// <summary>
        /// The cost of the tax to pay.
        /// </summary>
        public decimal? TaxTotal { get; set; }

        /// <summary>
        /// The cost of the tax to pay, formatted with currency.
        /// </summary>
        public string DisplayTaxTotal { get; set; }
    }
}
