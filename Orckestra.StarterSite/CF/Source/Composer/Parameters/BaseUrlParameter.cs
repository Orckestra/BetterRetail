using System;
using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class BaseUrlParameter
    {
        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        public string ReturnUrl { get; set; }

    }
}