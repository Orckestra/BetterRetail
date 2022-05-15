using System;

namespace Orckestra.Composer.Grocery.Settings
{
    public interface IMyUsualsSettings
    {
        Guid MyUsualsPageId { get; }
        int Frequency { get; }
        int TimeFrame { get; }
    }
}
