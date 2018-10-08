using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCartParam
    {
        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The unique identifier of the Customer owning the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

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
