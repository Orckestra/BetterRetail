using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CheckoutNavigationViewModel : BaseViewModel
    {
        //The List of the Checkout step
        public List<CheckoutNavigationStepViewModel> Steps { get; set; }

        public CheckoutNavigationViewModel()
        {
            Steps = new List<CheckoutNavigationStepViewModel>();
        }
    }

    public sealed class CheckoutNavigationStepViewModel : BaseViewModel
    {
        /// <summary>
        /// The Checkout step title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Checkout step Url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicate if it is the current step.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicate if the user can click to go back on this step.
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// The step number.
        /// </summary>
        public int StepNumber { get; set; }
    }
}
