using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class MonerisAddVaultProfileViewModel : BaseViewModel
    {
        /// <summary>
        /// Determines if the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Code of the error.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Message given with the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Active Payment ViewModel.
        /// </summary>
        public ActivePaymentViewModel ActivePayment { get; set; }
    }
}
