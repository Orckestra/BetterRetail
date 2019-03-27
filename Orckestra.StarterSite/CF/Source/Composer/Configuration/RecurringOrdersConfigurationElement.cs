using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Configuration
{
    public class RecurringOrdersConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "recurringOrders";


        public const string EnabledKey = "enabled";

        /// <summary>
        /// Determine if the website will show the recurring order features
        /// </summary>
        [ConfigurationProperty(EnabledKey, DefaultValue = "False", IsRequired = false)]
        public bool Enabled
        {
            get { return (bool)this[EnabledKey]; }
        }
    }
}
