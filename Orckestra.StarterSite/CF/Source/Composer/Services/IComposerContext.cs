using System;
using System.Globalization;

namespace Orckestra.Composer.Services
{
    public interface IComposerContext
    {
        CultureInfo CultureInfo { get; set; }
        string Scope { get; set; }
        Guid WebsiteId { get; set; }
        Guid CustomerId { get; set; }
        bool IsGuest { get; set; }
        string CountryCode { get; }
        bool IsAuthenticated { get; }
        string GetEncryptedCustomerId();
    }
}
