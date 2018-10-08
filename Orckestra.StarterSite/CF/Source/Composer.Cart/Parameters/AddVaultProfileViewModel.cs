using System;
using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Parameters
{
    public sealed class AddVaultProfileViewModel: BaseViewModel
    {
        /// <summary>
        /// Gets or sets the vault token identifier.
        /// </summary>
        /// <value>
        /// The vault token identifier.
        /// </value>
        [Required(AllowEmptyStrings = false)]
        public string VaultTokenId { get; set; }

        /// <summary>
        /// Gets or sets the name of the card holder.
        /// </summary>
        /// <value>
        /// The name of the card holder.
        /// </value>
        [Required(AllowEmptyStrings = false)]
        public string CardHolderName { get; set; }

        /// <summary>
        /// Gets or sets the payment identifier.
        /// </summary>
        /// <value>
        /// The payment identifier.
        /// </value>
        [Required]
        public Guid? PaymentId { get; set; }

        /// <summary>
        /// Flag wether a payment profile will be created or not following the checkout
        /// </summary>
        public bool CreatePaymentProfile { get; set; }

        /// <summary>
        /// Indicate the payment provider name of the profile we save
        /// </summary>
        /// <remarks>
        /// Used for cache invalidation
        /// </remarks>
        [Required]
        public string PaymentProviderName { get; set; }
    }
}
