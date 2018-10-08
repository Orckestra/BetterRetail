using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Utils
{
    public static class Guard
    {
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotNullOrWhiteSpace(string param, string paramName)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException($"{paramName} must not be null or whitespace.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> param, string paramName)
        {
            if (param == null || !param.Any())
            {
                throw new ArgumentException($"{paramName} must not be null or empty.", paramName);
            }
        }
    }
}