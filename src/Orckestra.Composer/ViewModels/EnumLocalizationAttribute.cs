using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EnumLocalizationAttribute: Attribute
    {
        public string LocalizationCategory { get; set; }
        /// <summary>
        /// By default if it doesn't find any value in the localization provider it fallbacks to the enum value. 
        /// If this property is set to true then it ignores the fallback behavior 
        /// </summary>
        public bool AllowEmptyValue { get; set; }

        public EnumLocalizationAttribute()
        {
            AllowEmptyValue = false;
        }
    }
}
