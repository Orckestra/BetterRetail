using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace Hangfire.CompositeC1.Types
{
    [DataScope(DataScopeIdentifier.PublicName)]
    [AutoUpdateble]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IExpirable : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("73c916a8-55be-496d-a72b-1c86057568df")]
        DateTime? ExpireAt { get; set; }
    }
}
