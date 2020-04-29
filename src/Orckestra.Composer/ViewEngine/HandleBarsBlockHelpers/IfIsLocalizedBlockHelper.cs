using System;
using System.IO;
using System.Threading;
using HandlebarsDotNet;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers
{
    internal class IfIsLocalizedBlockHelper : IHandlebarsBlockHelper
    {
        private readonly ILocalizationProvider _localizationProvider;

        public IfIsLocalizedBlockHelper(ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public string HelperName { get { return "if_localized"; } }

        public void HelperFunction(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException(string.Format("{{{{{0}}}}} helper must have exactly two arguments", HelperName));
            }

            string category = (arguments[0] ?? string.Empty).ToString();
            string key      = (arguments[1] ?? string.Empty).ToString();
            
            string resourceValue = _localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                CultureInfo = Thread.CurrentThread.CurrentCulture,
                Category    = category,
                Key         = key
            });

            if (!string.IsNullOrEmpty(resourceValue))
            {
                options.Template(output, context);
            }
            else
            {
                options.Inverse(output, context);
            }
        }
    }
}
