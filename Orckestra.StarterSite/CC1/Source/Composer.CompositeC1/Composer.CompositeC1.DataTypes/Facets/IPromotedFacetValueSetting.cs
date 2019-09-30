using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.Facets
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("1423b4f2-e7f4-48f0-a423-90d9046bd2ae")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Promoted Facet Value")]
    [LabelPropertyName("Title")]
    public interface IPromotedFacetValueSetting : IData
    {
        [ImmutableFieldId("c991cf0e-df89-4bfa-8a4a-47158eb5a4d1")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("28c8f8ec-afda-4658-9c59-e821e5068755")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        [FormRenderingProfile(Label = "Title")]
        string Title { get; set; }
        
        [ImmutableFieldId("7f3ccc78-33d0-4567-a14f-715f3fdeb9a0")]
        [FormRenderingProfile(Label = "Sort Weight")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
                                                   "nstant.Decimal\"><f:param name=\"Constant\" value=\"0\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Decimal, 28, 2)]
        [FieldPosition(1)]
        [DecimalPrecisionValidator(2)]
        [DefaultFieldDecimalValue(0)]
        [TreeOrdering(1, true)]
        decimal SortWeight { get; set; }
    }
}