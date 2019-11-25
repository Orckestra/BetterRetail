using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HandlebarsDotNet;

namespace Orckestra.Composer.ViewEngine.HandleBarsHelpers
{
    public class LookupHelper: IHandlebarsHelper
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.None);

        public string HelperName { get { return "lookup"; }}


        public void HelperFunction(TextWriter output, object context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException(string.Format("Helper {0} requires two parameters. Please visit {1} for more information.",
                 HelperName, "http://handlebarsjs.com/builtin_helpers.html#lookup"));
            }

            var collection = (IEnumerable<object>) arguments[0];
            int index = -1;
            if (!TryGetIndex(arguments[1].ToString(), out index))
            {
                throw new HandlebarsException("Unable to parse the index parameter. Please make sure its an integer");
            }

            var result = collection.ElementAtOrDefault(index);
            output.Write(result);
        }

        private bool TryGetIndex(string memberName, out int index)
        {
            var match = IndexRegex.Match(memberName);

            if (match.Success)
            {
                if (match.Groups["index"].Success)
                {
                    var indexGroup = match.Groups["index"].Value;
                    var success = int.TryParse(indexGroup, out index);
                    return success;
                }
            }

            index = 0;
            return false;
        }
    }
}
