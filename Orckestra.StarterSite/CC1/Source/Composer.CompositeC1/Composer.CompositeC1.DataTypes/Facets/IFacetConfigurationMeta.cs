using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.Facets
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("f03b2fd9-f551-4970-9331-6d5c6f25720b")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Facet Configuration")]
    [LabelPropertyName("Configuration")]
    public interface IFacetConfigurationMeta : IData, IPageMetaData, IPageData, IPageRelatedData, IPublishControlled, IProcessControlled, IVersioned
    {
        [ImmutableFieldId("9680b32f-31fa-412d-a081-c517218fdb49")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable=true)]
        [FieldPosition(0)]
        [ForeignKey("Orckestra.Composer.CompositeC1.DataTypes.Facets.IFacetConfiguration,Orckestra.Com" +
                    "poser.CompositeC1.DataTypes", AllowCascadeDeletes=true)]
        Nullable<Guid> Configuration { get; set; }
    }
}