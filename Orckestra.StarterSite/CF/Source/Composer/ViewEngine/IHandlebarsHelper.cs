using System.IO;

namespace Orckestra.Composer.ViewEngine
{
    /// <summary>
    /// Registrable Handlerbars Helper
    /// <see cref="HandlebarsDotNet.Handlebars.RegisterHelper" />
    /// </summary>
    public interface IHandlebarsHelper
    {
        /// <summary>
        /// TagName of the helper as exposed to handlebars
        /// <example>{{tagname A1 A2 A3}}</example>
        /// </summary>
        string HelperName { get; }

        /// <summary>
        /// Delegete methode called by Handlersbar when resolving the helper
        /// </summary>
        /// <param name="output"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        void HelperFunction(TextWriter output, object context, params object[] arguments);
    }
}
