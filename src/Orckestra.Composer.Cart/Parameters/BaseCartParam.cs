using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public abstract class BaseCartParam
    {
        /// <summary>
        /// The cart type
        /// </summary>
        public string CartType { get; set; }
    }
}