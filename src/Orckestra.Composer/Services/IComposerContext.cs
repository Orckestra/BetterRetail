﻿using System;
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
        string EditingOrderScope { get; set; }
        string EditingOrderId { get; set; }
        string EditingOrderNumber { get; set; }
        bool IsEditingOrder { get; }
    }
}
