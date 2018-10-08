using System;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Orckestra.Composer.CompositeC1.DataTypes.Navigation
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("119c7df0-0454-481b-b9be-a93b8a3bb01f")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Image of menu's item")]
    [LabelPropertyName("ImageSource")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface NavigationImage : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("7884ea1a-eb36-4de1-a462-edf79cbd9d9b")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("ff350431-65df-490e-a36c-42d66ac0d63d")]
        [FormRenderingProfile(Label = "Image Source", HelpText = " Specifies the image displayed. Image size must match the recommended size required by the website design. Image is never displayed on mobile and on tablet image is resized to fist the tablet view."
           , WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 2048)]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes = true, NullableString = true)]
        string ImageSource { get; set; }


        [ImmutableFieldId("f3a4afc6-2ceb-4b29-a8fd-5d0cf9e24255")]
        [FormRenderingProfile(Label = "Display On", HelpText = "Specifies the menu item the image is displayed. For example, select 'Men' to display this image when the 'Men' level one menu item is displayed.  Select 'Men > Shoes' to display this image when the 'Men > Shoes' level two menu item is displayed.")]
        [GuidNotEmpty]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(2)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(MainMenu), nameof(MainMenu.Id), AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
        Guid MenuItemParentId { get; set; }

     
        [ImmutableFieldId("c9b4589f-4813-48ee-902a-e116b3474e8d")]
        [FormRenderingProfile(Label = "Image Url", HelpText = "Specifies the URL used to redirect the user when he clicks on the image. The link can be a page in your website (internal link) or any external page.",
            WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.UrlComboBox"" />")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FieldPosition(3)]
        [NullStringLengthValidator(0, 256)]
        string ImageUrl { get; set; }


        [ImmutableFieldId("760cecaf-9bb0-4cb4-972c-b12574d2df49")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Orckestra.Co" +
         "mposer.CompositeC1.DataTypes.UrlTarget.GetDataReference\"><f:param name=\"KeyValue" +
         "\" value=\"d5203756-52fe-413e-a8ea-1ddacbf6549b\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FormRenderingProfile(Label = "Open In", HelpText = "Specifies how the page is displayed. For example, open in the current window.")]
        [GuidNotEmpty]
        [FieldPosition(4)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(UrlTarget), nameof(UrlTarget.Id), AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
        Guid Target { get; set; }


        [ImmutableFieldId("59155f27-59f5-4920-b26d-37b04ae9fe8c")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128, IsNullable = true)]
        [FormRenderingProfile(Label = "Image Caption", HelpText = "Specifies the image caption that will be shown when you have a mouse over the image.")]
        [FieldPosition(5)]
        [NullStringLengthValidator(0, 128)]
        string ImageLabel { get; set; }
        
    }
}