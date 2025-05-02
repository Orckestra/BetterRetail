using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("8ebd0140-cd52-4b31-b2f8-5f6717d26ba7")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Product Details Page Settings")]
    [LabelPropertyName("VideosInSummaryEnabled")]
    public interface ProductDetailsPageSettings : IPageMetaData, ILocalizedControlled
    {
        [ImmutableFieldId("5ffb0d03-2efe-4ef5-b731-08e85e40c2d9")]
        [FormRenderingProfile(Label = "Are videos displayed in the summary?")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
    "nstant.Boolean\"><f:param name=\"Constant\" value=\"False\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(0)]
        [DefaultFieldBoolValue(false)]
        bool VideosInSummaryEnabled { get; set; }
    }
}
