using Orckestra.Composer.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Utils
{
    public static class ConfigurationUtil
    {
        private static readonly RecurringOrdersConfigurationElement _recurringOrdersConfig;

        static ConfigurationUtil()
        {
            var conf = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;
            var confComposer = conf ?? new ComposerConfigurationSection();

            _recurringOrdersConfig = confComposer.RecurringOrdersConfiguration ?? new RecurringOrdersConfigurationElement();
        }

        public static bool GetRecurringOrdersConfigEnabled()
        {
            return _recurringOrdersConfig.Enabled;
        }
    }
}
