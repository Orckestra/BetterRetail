using System;
using System.IO;
using HandlebarsDotNet;

namespace Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers
{
    /// <summary>
    /// Template Block Helper that can be reused by multiple mathematical logic helpers
    /// to compare two arguments
    /// </summary>
    internal abstract class CompareBlockHelper<T>: IHandlebarsBlockHelper
    {
        public abstract string HelperName { get; }

        protected abstract bool Compare(T a, T b);
        protected abstract bool TryParse(object o, out T v);

        public void HelperFunction(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException(string.Format("{{{{{0}}}}} helper must have exactly two arguments", HelperName));
            }

            if (!TryParse(arguments[0], out T v1))
            {
                throw new ArgumentException(string.Format("The first argument of {{{{{0}}}}} helper must be parseable as a {1}", HelperName, typeof(T).Name));
            }

            if (!TryParse(arguments[1], out T v2))
            {
                throw new ArgumentException(string.Format("The second parameter of {{{{{0}}}}} helper must be parseable as a {1}", HelperName, typeof(T).Name));
            }

            if (Compare(v1, v2))
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
