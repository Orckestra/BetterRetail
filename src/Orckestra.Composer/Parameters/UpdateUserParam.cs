using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.Parameters
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

        public UpdateUserParam Clone()
        {
            var param = (UpdateUserParam)MemberwiseClone();
            return param;
        }
    }
}
