using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using Composite.Data.Validation.Validators;

namespace Orckestra.Composer.CompositeC1.DataTypes.Facets
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("a7f9d436-908f-4840-b2e6-216971a930db")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Facet Configuration")]
    [LabelPropertyName("Name")]
    public interface IFacetConfiguration : IData
    {
        [ImmutableFieldId("c4c4b1c7-c57f-4eab-9f87-8691e8539ea0")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("59ae7846-d2f6-45d3-b49e-749769a565a5")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        [FormRenderingProfile(Label = "Name")]
        string Name { get; set; }
        
        [ImmutableFieldId("2261093d-ae92-41fd-a367-bea978a1f4f0")]
        [FormRenderingProfile(Label = "Facets", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.DataIdMultiSelector""><f:param name=""OptionsType"" value=""Orckestra.Composer.CompositeC1.DataTypes.Facets.IFacet"" /><f:param name=""CompactMode"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(1)]
        [DefaultFieldStringValue("")]
        string Facets { get; set; }

        [ImmutableFieldId("4ef0be77-ce76-4698-b069-7458ef289ec5")]
        [FormRenderingProfile(Label = "Is Default", HelpText = "Determines if this Facet Category will be used as default")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
                                                   "nstant.Boolean\"><f:param name=\"Constant\" value=\"False\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(2)]
        [DefaultFieldBoolValue(false)]
        bool IsDefault { get; set; }
    }
}
