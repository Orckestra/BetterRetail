using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using Orckestra.Composer.Logging;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Services
{
    /// <summary>
    /// Gets settings from the Configuration File.
    /// </summary>
    public class SettingsFromConfigFileService : ISettingsService
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Gets an appSetting from the configuration.
        /// </summary>
        /// <typeparam name="T">Expected type for the given setting.</typeparam>
        /// <param name="settingName">Name of the setting to retrieve. If this setting does not exists, throws a <see cref="ConfigurationErrorsException"/></param>
        /// <returns></returns>
        public T GetAppSetting<T>(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(settingName)); }

            if (!IsSettingInAppSettings(settingName))
            {
                throw new ConfigurationErrorsException(string.Format("Cannot find AppSettings '{0}'", settingName));
            }

            var rawValue = GetSettingFromAppSettings(settingName);
            var value = ParseStringTo<T>(rawValue);
            return value;
        }

        /// <summary>
        /// Gets an appSetting from the configuration. If the settings does not exist, default value of <see cref="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">Expected type for the given setting.</typeparam>
        /// <param name="settingName">Name of the setting to retrieve.</param>
        /// <returns></returns>
        public T GetAppSettingOrDefault<T>(string settingName)
        {
            try
            {
                var value = GetAppSetting<T>(settingName);
                return value;
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.ErrorException(ex.Message, ex);               

                return default;
            }
        }

        public T GetSettingsSection<T>(string sectionName) 
            where T : ConfigurationSection
        {
            var section = ConfigurationManager.GetSection(sectionName);
            return (T)section;
        }

        /// <summary>
        /// Determines if a setting is in the AppSettings section of the Configuration File.
        /// </summary>
        /// <param name="settingName">Setting name</param>
        /// <returns></returns>
        private bool IsSettingInAppSettings(string settingName)
        {
            return WebConfigurationManager.AppSettings.AllKeys.Contains(settingName, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the string value of the AppSettings section.
        /// </summary>
        /// <param name="settingName">Setting name</param>
        /// <returns></returns>
        private string GetSettingFromAppSettings(string settingName)
        {
            return WebConfigurationManager.AppSettings.Get(settingName);
        }

        /// <summary>
        /// Parses a string value into <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to convert the string to.</typeparam>
        /// <param name="rawValue">String value to convert.</param>
        /// <returns></returns>
        private T ParseStringTo<T>(string rawValue)
        {
            return (T)Convert.ChangeType(rawValue, typeof (T));
        }
    }
}
