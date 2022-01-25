namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveInvalidLineItemsParam : BaseCartParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Whether or not to execute the specified workflow before returning the cart
        /// Optional
        /// </summary>
        public bool? ExecuteWorkflow { get; set; }

        /// <summary>
        /// The name of the workflow that should be executed before sending the cart back
        /// Optional
        /// </summary>
        public string WorkflowToExecute { get; set; }
    }
}