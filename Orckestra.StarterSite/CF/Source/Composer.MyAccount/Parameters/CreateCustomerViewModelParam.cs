using System.Globalization;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateCustomerViewModelParam
    {
        /// <summary>
        /// 
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
