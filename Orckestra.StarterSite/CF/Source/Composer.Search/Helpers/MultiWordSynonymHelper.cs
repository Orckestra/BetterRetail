using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orckestra.Composer.Search.Helpers
{
	//A list of multi word exceptions that should have their spaces replaced with dashes("-")

    public static class MultiWordSynonymHelper
    {
        public static string ExceptionMapOutput(string keyword, string cultureInfoName)
        {
            if (keyword.Split(' ').Length > 1)
            {
                List<Regex> value;
                if (exceptions.TryGetValue(cultureInfoName, out value))
                {
                    value.ForEach((exception) =>
                    {
                        if (exception.Match(keyword).Success)
                        {
                            keyword = exception.Replace(keyword, "-");
                        }
                    });
                }
            }

            return keyword;
        }

        private static Regex RegexRule(string str)
        {
            string[] parts = str.Split(' ');
            if (parts.Length == 2)
            {
                return new Regex($"(?<=(^|[\\s]){parts[0]})[\\W_]+(?={parts[1]}($|[\\s]))");
            }
            else if (parts.Length == 3)
            {
                return new Regex($"(?<=(^|[\\s]){parts[0]})[\\W_]+{parts[1]}[\\W_]+(?={parts[2]}($|[\\s]))");
            }
            else
            {
                throw new Exception("Exceptions must have only 2 or 3 words separated by whitespace.");
            }
        }

		//This list is an example from Sports Experts....we probably want to make our own (and make it configurable)
        private static Dictionary<string, List<Regex>> exceptions = new Dictionary<string, List<Regex>>()
        {
            {
                "en-US",
                new List<string> {
                  // Sports
                  "ping pong",
                  "volley ball",
                  "volley bal",
                  "voley bal",
                  "voley ball",
                  "basket ball",
                  "basket bal",
                  "base ball",
                  "base bal",
                  // Brand
                  "o neill",
                  "o neil",
                  "under armor",
                  "hunder armour",
                  "under amour",
                  "arc teryx",
                  "skate board",
                  "skate baord",
                  // Apparel
                  "fit bit"
                }.Select(RegexRule).ToList()
            }
        };
    }
}
