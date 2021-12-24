using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.Grocery.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("f00f2972-27da-4c7c-9f35-32fc2e2aef92")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("PromotionalRibbon")]
    [LabelPropertyName("LookupValue")]
    [SearchableType]
    public interface IPromotionalRibbonConfiguration : IData
    {
        [ImmutableFieldId("7ad1a538-a357-40dc-b8df-332021f5a213")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
           "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("01f8fdbb-87cc-4bce-8da1-ac58dfb0781b")]
        [StoreFieldType(PhysicalStoreFieldType.String, 16)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 16)]
        [DefaultFieldStringValue("")]
        [FormRenderingProfile(Label = "Promotional Ribbon Lookup Value", HelpText = "Specify the value which equals to the Promotional Ribbon lookup value you want to configure.")]
        string LookupValue { get; set; }

        [ImmutableFieldId("52129127-4367-4751-a86c-465db9f09419")]
        [FormRenderingProfile(Label = "Background Color", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/BackgroundOptions.xml"" /></f:function></f:param></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string BackgroundColor { get; set; }

        [ImmutableFieldId("db08774b-e6c4-4042-a2a3-c70cb2d31591")]
        [FormRenderingProfile(Label = "Text Color", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/TextColorOptions.xml"" /></f:function></f:param></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(2)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string TextColor { get; set; }

    }
}
