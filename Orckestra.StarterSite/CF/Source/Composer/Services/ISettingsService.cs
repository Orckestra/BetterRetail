using System.Configuration;

namespace Orckestra.Composer.Services
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets an appSetting from the configuration.
        /// </summary>
        /// <typeparam name="T">Expected type for the given setting.</typeparam>
        /// <param name="settingName">Name of the setting to retrieve. If this setting does not exists, throws a <see cref="ConfigurationErrorsException"/></param>
        /// <returns></returns>
        T GetAppSetting<T>(string settingName);

        /// <summary>
        /// Gets an appSetting from the configuration. If the settings does not exist, default value of <see cref="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">Expected type for the given setting.</typeparam>
        /// <param name="settingName">Name of the setting to retrieve.</param>
        /// <returns></returns>
        T GetAppSettingOrDefault<T>(string settingName);

        /// <summary>
        /// Obtains an instance of the requested configuration section.
        /// </summary>
        /// <returns></returns>
        T GetSettingsSection<T>(string sectionName) 
            where T : ConfigurationSection;
    }
}