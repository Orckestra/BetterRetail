using System.IO;
using HandlebarsDotNet;

namespace Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers
{
    internal class IfExistsBlockHelper : IHandlebarsBlockHelper
    {
        public string HelperName { get { return "if_exists"; } }

        public void HelperFunction(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException(string.Format("{{{{{0}}}}} helper must have exactly one argument", HelperName));
            }
            if (arguments[0] != null)
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
