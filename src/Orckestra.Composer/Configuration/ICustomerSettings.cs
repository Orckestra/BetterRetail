using System.Threading.Tasks;
using ProfileSettings = Orckestra.Overture.ServiceModel.Customers.ProfileSettings;

namespace Orckestra.Composer.Configuration
{
    public interface ICustomerSettings
    {
        Task<ProfileSettings> GetProfileSettingsAsync();
    }
}
