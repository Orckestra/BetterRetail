using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Orckestra.Composer.Grocery.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("f34ff3e6-12fa-49cb-afd4-5ca9001c7eb5")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("PromotionalBanner")]
    [LabelPropertyName("LookupValue")]
    [SearchableType]
    public interface IPromotionalBannerConfiguration : IData
    {
        [ImmutableFieldId("222c4e0f-1454-48e4-a07a-186226ae731c")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
           "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("b4a395a1-14d9-434c-a47d-77df9c9a0b9c")]
        [StoreFieldType(PhysicalStoreFieldType.String, 16)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 16)]
        [DefaultFieldStringValue("")]
        [FormRenderingProfile(Label = "Promotional Banner Lookup Value", HelpText = "Specify the value which equals to the Promotional Banner lookup value you want to configure.")]
        string LookupValue { get; set; }

        [ImmutableFieldId("5f771cf8-336c-4544-9bc3-c8de7a62c452")]
        [FormRenderingProfile(Label = "Background Color", HelpText = "If none is selected, then default Dark backgroud is used.", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/BackgroundOptions.xml"" /></f:function></f:param></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string BackgroundColor { get; set; }

        [ImmutableFieldId("a1d00f49-6e3c-4a32-a4da-66582a8b9ce6")]
        [FormRenderingProfile(Label = "Text Color", HelpText = "If none is selected, then default White color is used.", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Web.Html.GetStyleOptionsFromFile""><f:param name=""OptionsXMLFilePath"" value=""UI.Package/GetStyleOptionsFromFile/TextColorOptions.xml"" /></f:function></f:param></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(2)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string TextColor { get; set; }
    }
}
