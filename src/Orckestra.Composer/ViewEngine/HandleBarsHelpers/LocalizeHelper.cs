using System;
using System.IO;
using System.Threading;
using HandlebarsDotNet;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.ViewEngine.HandleBarsHelpers
{
    internal class LocalizeHelper : IHandlebarsHelper
    {
        private readonly ILocalizationProvider _localizationProvider;

        public LocalizeHelper(ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public string HelperName { get { return "localize"; } }

        public void HelperFunction(TextWriter output, object context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException(string.Format("{{{{{0}}}}} helper must have exactly two arguments", HelperName));
            }

            var parms = new GetLocalizedParam
            {
                CultureInfo = Thread.CurrentThread.CurrentCulture,
                Category    = (arguments[0] ?? string.Empty).ToString(),
                Key         = (arguments[1] ?? string.Empty).ToString(),
            };
            
            string resourceValue = _localizationProvider.GetLocalizedString(parms);

            if (resourceValue == null) // Do not use !IsNullOrWhiteSpace, user might want to display empty string.
            {
                resourceValue = GetMissingValueHint(parms);
            }

            output.WriteSafeString(resourceValue);
        }
        
        /// <summary>
        /// Format the requested Key to be displayed as a hint on missing localization
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string GetMissingValueHint(GetLocalizedParam param)
        {
            return string.Format("[{0}.{1}]", param.Category, param.Key);
        }
    }
}
