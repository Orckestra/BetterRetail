using System;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    [Flags]
    public enum CacheDependentOperations
    { 
        Add     = 0b_0001,
        Update  = 0b_0010,
        Deleted = 0b_0100,
        All     = Add | Update | Deleted,
    };
}