using System;
using System.Globalization;

namespace Orckestra.Composer.Services
{
    public interface IComposerContext
    {
        CultureInfo CultureInfo { get; set; }
        string Scope { get; }
        Guid CustomerId { get; set; }
        bool IsGuest { get; set; }
        string CountryCode { get; }
        bool IsAuthenticated { get; }
        string GetEncryptedCustomerId();
        string ScopeCurrencyIso { get; }
        string EditingCartName { get; set; }
        string EditingScopeId { get; set; }
        int LocalTimeZoneOffset { get; }
    }
}
