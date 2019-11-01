using System;
using System.Globalization;

namespace Orckestra.Composer.Services
{
    public interface IComposerRequestContext
    {
        CultureInfo CultureInfo { get; set; }
        string Scope { get; }
        Guid CustomerId { get; set; }
        bool IsGuest { get; set; }
        string CountryCode { get; }
        bool IsAuthenticated { get; }
        string GetEncryptedCustomerId();
    }
}
