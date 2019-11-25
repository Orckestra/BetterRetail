using System.IO;
using HandlebarsDotNet;

namespace Orckestra.Composer.ViewEngine
{
    /// <summary>
    /// Registrable Handlerbars Helper
    /// <see cref="HandlebarsDotNet.Handlebars.RegisterHelper" />
    /// </summary>
    public interface IHandlebarsBlockHelper
    {
        /// <summary>
        /// TagName of the helper as exposed to handlebars
        /// <example>{{#tagname A1}}Primary{else}Inverse{/tagname}</example>
        /// </summary>
        string HelperName { get; }

        /// <summary>
        /// Delegete methode called by Handlersbar when resolving the helper
        /// </summary>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        void HelperFunction(TextWriter output, HelperOptions options, object context, params object[] arguments);
    }
}
