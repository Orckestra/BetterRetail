using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Orckestra.Composer.Utils
{
    /// <summary>
    /// Class designed for expressions transformations and presentations
    /// </summary>
    public static class ExpressionUtility
    {
        /// <summary>
        /// Get a string presentation of a code included to an expression.
        /// </summary>
        /// <param name="expression">
        /// Expression to be processed which belongs to <see cref="Expression"/> class.  
        /// </param>
        /// <example>
        /// <code>
        /// var exampleString = ExpressionUtility.GetExpressionCodeAsString(()=>string.Empty);
        /// </code>
        /// The value of the exampleString variable will be "string.Empty".
        /// </example>
        /// <returns>
        /// String presentation of a code included to an expression.
        /// </returns>
        public static string GetExpressionCodeAsString<T>(Expression<Func<T>> expression)
        {
            string value = expression?.Body?.ToString();
            if (string.IsNullOrEmpty(value)) { return null; }
            Match match = Regex.Match(value, "value\\(.+?\\).(.+)");
            return match.Success ? match.Groups[1].Value : value;
        }

        /// <summary>
        /// Get a ParametherInfo array of a method call expression
        /// </summary>
        /// <param name="expression">Method call expression</param>
        /// <returns>ParameterInfo array</returns>
        public static ParameterInfo[] GetParamsInfo<T>(Expression<Func<T>> expression)
        {
            MethodInfo method = ((MethodCallExpression)expression.Body).Method;
            return method.GetParameters();
        }

    }
}