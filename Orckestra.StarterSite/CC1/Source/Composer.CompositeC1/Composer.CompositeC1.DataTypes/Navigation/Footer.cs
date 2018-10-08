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
    [ImmutableTypeId("3887c4c9-2667-4744-bd65-a9b4e79e0495")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Footer Item")]
    [LabelPropertyName("DisplayName")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface Footer : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("07c0e6f6-8808-4ece-adcf-09c1412682eb")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("3efa290f-5db8-4dfe-ac99-4297d9038584")]
        [FormRenderingProfile(Label = "Display Name", HelpText = "Displays the name of the menu item. The name must be short. Use text like 'Women', 'Brands', 'Boys shoes' etc.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string DisplayName { get; set; }

        [ImmutableFieldId("b30431d5-97d0-4a89-906c-4b2a3274682c")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FormRenderingProfile(Label = "Parent Footer Item", HelpText = "Specifies under which footer item the item must be displayed. For example: for 'Men shoes',  select 'Men' as the parent footer item to display 'Men shoes' as a sub-category element.")]
        [FieldPosition(1)]
        [ForeignKey(typeof(Footer), nameof(Footer.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> ParentId { get; set; }

        [ImmutableFieldId("5b3259e4-6ef7-44a8-b518-9bdb9d077d17")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.UrlComboBox"" />",
                        Label = "Link", HelpText = "Specifies the URL of the menu item. The link can be a page from your website (internal link) or any external page that you want to display on the website header.  When linked to your web site page, the link will be displayed only if the page is published and live.")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128, IsNullable = true)]
        [FieldPosition(2)]
        [NullStringLengthValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Url { get; set; }


        [ImmutableFieldId("fe09ad88-e220-4f75-86e9-e5134ede38c0")]
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

        [ImmutableFieldId("3523b2b2-3c31-4fc1-8abb-5b68df6741eb")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"1\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FormRenderingProfile(Label = "Display Order", HelpText = "Specifies the position of this menu item in the menu. An item with order value of 5 will be displayed as the fifth item of the menu within the level of this item.")]
        [FieldPosition(4)]
        [IntegerRangeValidator(-2147483648, 2147483647)]
        [DefaultFieldIntValue(0)]
        int Order { get; set; }
        
        [ImmutableFieldId("eeef1937-7c2b-48df-87cd-8ecfd6f352f1")]
        [FormRenderingProfile(Label = "Display Style", HelpText = "Specify the style used to display the item. You can enter a CSS class or inline styling e.g. style=color: blue;. When no style is defined, the optional link is displayed using the website default style.")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(6)]
        [ForeignKey(typeof(CssStyle), nameof(DataTypes.CssStyle.Id), AllowCascadeDeletes = true)]
        Nullable<Guid> CssStyle { get; set; }
  
    }
}