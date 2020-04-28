using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public interface IPaymentMethodViewModel
    {
        /// <summary>
        /// The Overture unique identifier of the Payment Method.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// The Overture unique identifier of the Payment Provider.
        /// </summary>
        string PaymentProviderName { get; set; }

        string PaymentProviderType { get; set; }

        /// <summary>
        /// The Name of the Payment Method.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Determines if the payment method is currently selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Determine if the payment method is the default one
        /// </summary>
        bool Default { get; set; }

        /// <summary>
        /// Indicate the payment type
        /// </summary>
        string PaymentType { get; set; }

        bool IsValid { get; }

        bool IsCreditCardPaymentMethod { get; set; }
    }
}
