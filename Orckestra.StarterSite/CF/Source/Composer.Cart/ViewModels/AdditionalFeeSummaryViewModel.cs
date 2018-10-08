using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class AdditionalFeeSummaryViewModel : BaseViewModel
    {
        /// <summary>
        /// The group name of additional fees
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The total amount for this group
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// Whether additional fee is taxable or not.
        /// </summary>
        public bool Taxable { get; set; }
    }
}
