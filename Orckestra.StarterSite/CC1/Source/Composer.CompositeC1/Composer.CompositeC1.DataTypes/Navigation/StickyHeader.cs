using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.Navigation
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("cb55cf71-40cd-4887-949c-c769e500d53d")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Sticky Header")]
    [LabelPropertyName("DisplayName")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface StickyHeader : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("3bc737bf-a6fd-4045-b952-333ef275d6b5")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("d4482de9-3f3f-44ce-9025-06b717e29ed0")]
        [FormRenderingProfile(Label = "Display Name", HelpText = "Displays the name of the menu item. The name must be short. Use text like 'Women', 'Brands', 'Boys shoes' etc.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string DisplayName { get; set; }

        [ImmutableFieldId("5afed821-a294-406f-a17e-c3d1fb30adb1")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.UrlComboBox"" />",
                        Label = "Link", HelpText = "Specifies the URL of the menu item. The link can be a page from your website (internal link) or any external page that you want to display on the website header.  When linked to your web site page, the link will be displayed only if the page is published and live.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Url { get; set; }


        [ImmutableFieldId("5aa3506b-8835-46ec-a48a-497417b8c85c")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Orckestra.Co" +
            "mposer.CompositeC1.DataTypes.UrlTarget.GetDataReference\"><f:param name=\"KeyValue" +
            "\" value=\"d5203756-52fe-413e-a8ea-1ddacbf6549b\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FormRenderingProfile(Label = "Open In", HelpText = "Specifies how the page is displayed. For example, open in the current window.")]
        [GuidNotEmpty]
        [FieldPosition(2)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(UrlTarget), nameof(UrlTarget.Id), AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
        Guid Target { get; set; }

        [ImmutableFieldId("3990009f-17c7-463f-b7fb-9d9a819247e0")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"1\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FormRenderingProfile(Label = "Display Order", HelpText = "Specifies the position of this menu item in the menu. An item with order value of 5 will be displayed as the fifth item of the menu within the level of this item.")]
        [FieldPosition(3)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        int Order { get; set; }

        [ImmutableFieldId("2aea131c-1ebe-4c39-8d82-a44f302bbf02")]
        [FormRenderingProfile(Label = "Display Style", HelpText = "Specify the style used to display the item. You can enter a CSS class or inline styling e.g. style=color: blue;. When no style is defined, the optional link is displayed using the website default style.")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(6)]
        [ForeignKey(typeof(CssStyle), nameof(DataTypes.CssStyle.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> CssStyle { get; set; }

    }
}