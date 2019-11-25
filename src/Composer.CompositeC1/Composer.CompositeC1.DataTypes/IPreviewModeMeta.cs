using System;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("3ea8c217-614f-4397-831e-7d6b314ff558")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Preview Mode")]
    [LabelPropertyName("ProductId")]
    public interface IPreviewModeMeta : IData, IPageMetaData, IPageData, IPageRelatedData, IPublishControlled, IProcessControlled, IVersioned
    {
        [ImmutableFieldId("7b1baab3-b83e-4dd5-3bf6-3a99da5a7cd3")]
        [FormRenderingProfile(Label = "Product ID")]
        [StoreFieldType(PhysicalStoreFieldType.String, 16, IsNullable = true)]
        [FieldPosition(0)]
        [NullStringLengthValidator(0, 16)]
        string ProductId { get; set; }
    };
}