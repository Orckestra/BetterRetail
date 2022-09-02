using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.OccClient
{
    [DataContract]
    public class CreditCardPaymentProperties
    {
        /// <summary>
        /// Property key for credit card number last digits.
        /// </summary>
        public static readonly string CreditCardNumberLastDigitsKey = "CreditCardNumberLastDigits";

        /// <summary>
        /// Property key for credit card expiry date.
        /// </summary>
        public static readonly string CreditCardExpiryDateKey = "CreditCardExpiryDate";

        /// <summary>
        /// Property key for credit card address verification result code.
        /// </summary>
        public static readonly string CreditCardAvsResultCode = "CreditCardAvsResultCode";

        /// <summary>
        /// Property key for credit card CVD verification result code.
        /// </summary>
        public static readonly string CreditCardCvdResultCode = "CreditCardCvdResultCode";

        /// <summary>
        /// Property key for credit card response code.
        /// </summary>
        public static readonly string CreditCardResponseCode = "CreditCardResponseCode";

        /// <summary>
        /// Property key for credit card brand.
        /// </summary>
        public static readonly string CreditCardBrandKey = "CreditCardBrand";

        /// <summary>
        /// Property key for credit card country.
        /// </summary>
        public static readonly string CreditCardCountryKey = "CreditCardCountry";
    }
}
