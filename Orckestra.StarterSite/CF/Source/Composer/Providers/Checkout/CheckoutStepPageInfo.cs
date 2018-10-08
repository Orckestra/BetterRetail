namespace Orckestra.Composer.Providers.Checkout
{
    public class CheckoutStepPageInfo
    {
        /// <summary>
        /// The url of the checkout step.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The Title of the checkout step.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Indicate if the step should be displayed in the header.
        /// </summary>
        public bool IsDisplayedInHeader { get; set; }
    }
}
