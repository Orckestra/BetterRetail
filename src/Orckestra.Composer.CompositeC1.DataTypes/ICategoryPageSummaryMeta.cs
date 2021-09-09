using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("856daf78-4637-4c81-b2e7-5c8be8ccf7b0")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Category Page Summary")]
    [LabelPropertyName("Title")]
    public interface ICategoryPageSummaryMeta : IData, ILocalizedControlled, IProcessControlled, IPageData, IPageRelatedData, IPageMetaData, IPublishControlled, IVersioned
    {
        [ImmutableFieldId("f4919cac-de62-48f9-9af7-d438c15f83b5")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128, IsNullable=true)]
        [FieldPosition(0)]
        [NullStringLengthValidator(0, 128)]
        string Title { get; set; }
        
        [ImmutableFieldId("9821c545-458d-459c-a939-715124bf8f98")]
        [FormRenderingProfile(Label = "Short Description")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable=true)]
        [FieldPosition(1)]
        [NullStringLengthValidator(0, 512)]
        string ShortDescription { get; set; }
        
        [ImmutableFieldId("35044fb1-aa5a-4920-87a7-6db24b8f3996")]
        [FormRenderingProfile(Label = "Long Description", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"" />")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.XhtmlDocument\"><f:param name=\"Constant\"><html xmlns=\"http://www.w3.org/19" +
            "99/xhtml\"><head /><body /></html></f:param></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(2)]
        [DefaultFieldStringValue("")]
        string LongDescription { get; set; }
        
        [ImmutableFieldId("3503d90f-ce92-48c0-8a9d-90acd2011976")]
        [FormRenderingProfile(Label = "Background Image", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048, IsNullable=true)]
        [FieldPosition(3)]
        [NullStringLengthValidator(0, 2048)]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes=true, NullableString=true)]
        string BackgroundImage { get; set; }
    }
}