using System;
using System.Globalization;
using System.IO;
using System.Threading;
using HandlebarsDotNet;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.ViewEngine.HandleBarsHelpers
{
    internal class FormatValueHelper : IHandlebarsHelper
    {
        public string HelperName { get { return "formatValue"; } }

        public void HelperFunction(TextWriter output, object context, params object[] arguments)
        {
            if (arguments.Length != 3)
            {
                throw new HandlebarsException($"{{{{{HelperName}}}}} helper must have exactly three arguments");
            }

            string format = "{" + (arguments[1] ?? string.Empty) + "}";

            var outputResponse = FormatValue(arguments[0], format,
                bool.TryParse((arguments[2] ?? string.Empty).ToString(), out bool useCurrentCulture) && useCurrentCulture);

            output.WriteSafeString(outputResponse);
        }

        private string FormatValue(object value, string format, bool useCurrentCulture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(format))
            {
                return Convert.ToString(value, useCurrentCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture);
            }
            else
            {
                return string.Format(useCurrentCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture, format, value);
            }
        }
    }
}
