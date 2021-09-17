using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.Grocery.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("5ce4bcd2-97af-4534-b3ad-51fd94de679b")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Bootstrap Carousel Items")]
    [LabelPropertyName("Heading")]
    [Caching(CachingType.Full)]
    public interface IBootstrapCarousel : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, ILocalizedControlled
    {
        [ImmutableFieldId("77e91867-049b-4baa-8b1b-d92a6cf56fd5")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("f859a4d6-f4df-4e72-9e1e-19bfe4026a89")]
        [FormRenderingProfile(Label = "Slide Image", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048, IsNullable=true)]
        [FieldPosition(0)]
        [NullStringLengthValidator(0, 2048)]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes=true, NullableString=true)]
        string Image { get; set; }
        
        [ImmutableFieldId("fa36fc57-5a19-4818-a7bb-9076f8b2094f")]
        [FormRenderingProfile(Label = "Slide Background Color", HelpText = "Can be used instead of Image.", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/BackgroundOptions.xml"" /></f:function></f:param><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(1)]
        [NullStringLengthValidator(0, 64)]
        string BackgroundColor { get; set; }
        
        [ImmutableFieldId("1b2689f9-898d-4e85-9bd8-404ba0e87967")]
        [FormRenderingProfile(Label = "Slide Heading")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(2)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Heading { get; set; }
        
        [ImmutableFieldId("2149d501-c4df-4d3f-bd0e-b2e2274a7542")]
        [FormRenderingProfile(Label = "Slide Heading Color", HelpText = "White by default", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/TextColorOptions.xml"" /></f:function></f:param><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(3)]
        [NullStringLengthValidator(0, 64)]
        string HeadingColor { get; set; }
        
        [ImmutableFieldId("8b2d0aaa-aab6-4d6b-8ff2-dadab4f9c3d6")]
        [FormRenderingProfile(Label = "Slide Heading Position", HelpText = "Center by defaukt", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/TextPositionOptions.xml"" /></f:function></f:param><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(4)]
        [NullStringLengthValidator(0, 64)]
        string HeadingPosition { get; set; }
        
        [ImmutableFieldId("c388dab1-f52d-4da2-b5a3-11ccaa05881c")]
        [FormRenderingProfile(Label = "Slide Caption", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"" />")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.XhtmlDocument\"><f:param name=\"Constant\"><html xmlns=\"http://www.w3.org/19" +
            "99/xhtml\"><head /><body /></html></f:param></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(5)]
        [DefaultFieldStringValue("")]
        string Caption { get; set; }
        
        [ImmutableFieldId("30bbb99b-e176-4351-b482-2d10232f05a5")]
        [FormRenderingProfile(Label = "Slide Caption Background Color", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/BackgroundOptions.xml"" /></f:function></f:param><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(6)]
        [NullStringLengthValidator(0, 64)]
        string CaptionBgColor { get; set; }
        
        [ImmutableFieldId("61713b7a-f8c2-4aef-91e9-0a9279ca687f")]
        [FormRenderingProfile(Label = "Slide Caption Background Color as RGBA or #")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable=true)]
        [FieldPosition(7)]
        [NullStringLengthValidator(0, 64)]
        string CaptionBgRGBAColor { get; set; }
        
        [ImmutableFieldId("dbc62c45-e131-4bc2-9262-5b5e4dd372f0")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable=true)]
        [FieldPosition(8)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        [TreeOrdering(1)]
        Nullable<int> Order { get; set; }
    }
}