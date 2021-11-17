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
    [ImmutableTypeId("223de5b5-49c3-4024-8767-16956f3a1ff1")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Main Menu Item")]
    [LabelPropertyName("DisplayName")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface MainMenu : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("93d54821-a46c-4524-9899-d5a94860b680")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("f26c291b-cb5b-4677-ae85-2ddfa60026e0")]
        [FormRenderingProfile(Label = "Display Name", HelpText = "Displays the name of the menu item. The name must be short. Use text like 'Women', 'Brands', 'Boys shoes' etc.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string DisplayName { get; set; }

        [ImmutableFieldId("22e5fb62-bba7-4b79-bd61-6e25f7231c4a")]
        [FormRenderingProfile(Label = "Parent Menu Item", HelpText = "Specifies under which menu item the item must be displayed. For example: for 'Men shoes',  select 'Men' as the parent menu item to display 'Men shoes' as a sub-category element.")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(1)]
        [ForeignKey(typeof(MainMenu), nameof(MainMenu.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> ParentId { get; set; }

        [ImmutableFieldId("aeeca1ba-507f-4775-822a-20aaded7eff1")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.UrlComboBox"" />",
            Label = "Link", HelpText = "Specifies the URL of the menu item. The link can be a page from your website (internal link) or any external page that you want to display on the website header.  When linked to your web site page, the link will be displayed only if the page is published and live.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FieldPosition(2)]
        [NullStringLengthValidator(0, 256)]
        [DefaultFieldStringValue("")]
        string Url { get; set; }
     

        [ImmutableFieldId("b21d8228-dfd2-4501-94fc-97e53594d383")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Orckestra.Co" +
            "mposer.CompositeC1.DataTypes.UrlTarget.GetDataReference\"><f:param name=\"KeyValue" +
            "\" value=\"d5203756-52fe-413e-a8ea-1ddacbf6549b\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FormRenderingProfile(Label = "Open In", HelpText = "Specifies how the page is displayed. For example, open in the current window.")]
        [GuidNotEmpty]
        [FieldPosition(3)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(UrlTarget), nameof(UrlTarget.Id), AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
        Guid Target { get; set; }

        [ImmutableFieldId("01c6f831-45f7-482c-a50a-9fed3d0babeb")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"1\" /></f:function>")]
        [FormRenderingProfile(Label = "Display Order", HelpText = "Specifies the position of this menu item in the menu. An item with order value of 5 will be displayed as the fifth item of the menu within the level of this item.")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(4)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        int Order { get; set; }
        
        [ImmutableFieldId("844ac3c5-beb3-4758-91e6-597f78e2377c")]
        [FormRenderingProfile(Label = "Display Style", HelpText = "Specify the style used to display the item. You can enter a CSS class or inline styling e.g. style=color: blue;. When no style is defined, the optional link is displayed using the website default style.")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(6)]
        [ForeignKey(typeof(CssStyle), nameof(DataTypes.CssStyle.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> CssStyle { get; set; }

        [ImmutableFieldId("775447a2-4009-4631-811d-71ce4ef85f1a")]
        [FormRenderingProfile(Label = "Css Class Name", HelpText = "Specify the css class name to apply to this item. Next predefined class names can be used: highlight, hide-on-scroll")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable = true)]
        [FieldPosition(7)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string CssClassName { get; set; }
    }
}