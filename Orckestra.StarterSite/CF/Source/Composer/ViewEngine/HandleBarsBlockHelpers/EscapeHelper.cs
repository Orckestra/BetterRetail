using System.IO;
using System.Text;
using System.Web;
using HandlebarsDotNet;

namespace Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers
{
    internal class EscapeHelper : IHandlebarsBlockHelper
    {
        public string HelperName { get { return "escape"; } }

        public void HelperFunction(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            var sBuilder = GetTemplateUnsafeContent(options, context);

            string safeStr = HttpUtility.HtmlEncode(sBuilder.ToString());
            output.WriteSafeString(safeStr);
        }

        private static StringBuilder GetTemplateUnsafeContent(HelperOptions options, object context)
        {
            var sBuilder = new StringBuilder();
            var writer = new StringWriter(sBuilder);

            options.Template.Invoke(writer, context);
            return sBuilder;
        }
    }
}
