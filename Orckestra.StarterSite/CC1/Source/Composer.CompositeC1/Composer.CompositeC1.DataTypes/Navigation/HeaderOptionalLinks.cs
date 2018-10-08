using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.CompositeC1.DataTypes.Navigation
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("08df5f3f-791e-411d-b33f-fd072ad23274")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Header Optional Links")]
    [LabelPropertyName("DisplayName")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface HeaderOptionalLink : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("4f7359c9-1782-4e21-b6ab-77647ed4a667")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("b8c9167c-ebd8-4e19-9e7a-7e3b17d498a9")]
        [FormRenderingProfile(Label = "Display Name", HelpText = "Displays the name of the menu item. The name must be short. Use text like 'Women', 'Brands', 'Boys shoes' etc.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string DisplayName { get; set; }

        [ImmutableFieldId("faadf24f-9dd3-4829-acb8-9bffc44a0280")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.UrlComboBox"" />",
                        Label = "Link", HelpText = "Specifies the URL of the menu item. The link can be a page from your website (internal link) or any external page that you want to display on the website header.  When linked to your web site page, the link will be displayed only if the page is published and live.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Url { get; set; }

        [ImmutableFieldId("b21d8228-dfd2-4501-94fc-97e53594d383")]
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

        [ImmutableFieldId("eba83041-de20-43a1-9a2a-450db4c21114")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"1\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FormRenderingProfile(Label = "Display Order", HelpText = "Specifies the position of this menu item in the menu. An item with order value of 5 will be displayed as the fifth item of the menu within the level of this item.")]
        [FieldPosition(3)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        int Order { get; set; }

        [ImmutableFieldId("e4b921fa-e773-445f-b0fb-1e4303c3f456")]
        [FormRenderingProfile(Label = "Display Style", HelpText = "Specify the style used to display the item. You can enter a CSS class or inline styling e.g. style=color: blue;. When no style is defined, the optional link is displayed using the website default style.")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(6)]
        [ForeignKey(typeof(CssStyle), nameof(DataTypes.CssStyle.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> CssStyle { get; set; }
    }
}