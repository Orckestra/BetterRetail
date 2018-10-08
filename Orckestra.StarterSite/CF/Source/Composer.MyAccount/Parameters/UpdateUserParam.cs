using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class UpdateUserParam
    {
        /// <summary>
        /// (Mandatory)
        /// The customer to update
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope in which the User will be updated
        /// </summary>
        public string Scope { get; set; }
    }
}
