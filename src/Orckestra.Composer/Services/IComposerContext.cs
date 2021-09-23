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
        string CurrencyIso { get; }
        string GetEncryptedCustomerId();
    }
}
