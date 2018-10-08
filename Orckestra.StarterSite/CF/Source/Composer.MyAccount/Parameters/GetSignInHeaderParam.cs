using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetSignInHeaderParam
    {
        public Guid CustomerId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string Scope { get; set; }
        public bool IsAuthenticated { get; set; }
        public string EncryptedCustomerId { get; set; }
    }
}
