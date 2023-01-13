using Orckestra.Composer.Repositories;
using System;
using System.Threading.Tasks;
using ProfileSettings = Orckestra.Overture.ServiceModel.Customers.ProfileSettings;

namespace Orckestra.Composer.Configuration
{
    public class CustomerSettings: ICustomerSettings
    {
        private static ProfileSettings _profileSettings;
        protected ICustomerSettingsRepository CustomerSettingsRepository { get; private set; }

        public CustomerSettings(ICustomerSettingsRepository customerSettingsRepository)
        {
            CustomerSettingsRepository = customerSettingsRepository ?? throw new ArgumentNullException(nameof(customerSettingsRepository));
        }
        public virtual async Task<ProfileSettings> GetProfileSettingsAsync()
        {
            if (_profileSettings == null)
            {
                _profileSettings = await CustomerSettingsRepository.GetCustomerSettings().ConfigureAwait(false);
            }

            return _profileSettings;
        }
    }
}
