using Orckestra.Overture.ServiceModel.Customers;
using System.Threading.Tasks;

namespace Orckestra.Composer.Repositories
{
    public interface ICustomerSettingsRepository
    {
        /// <summary>
        /// Retrieve the Customer profile Settings from Overture
        /// </summary>
        /// <returns>Profile Settings</returns>
        Task<ProfileSettings> GetCustomerSettings();
    }
}