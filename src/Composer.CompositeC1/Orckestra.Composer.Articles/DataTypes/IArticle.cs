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

namespace Orckestra.Composer.Articles.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("a2c3270e-9d6c-4473-901c-033f324a6777")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Articles")]
    [LabelPropertyName("Title")]
    [InternalUrl("articles")]
    [Caching(CachingType.Full)]
    [SearchableType]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    public interface IArticle : IData, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("35811703-36c6-43e3-9932-23defd5eb7aa")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("cb8f44ed-08d0-4189-bbfc-c90b7de40052")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, true, false)]
        string Title { get; set; }
        
        [ImmutableFieldId("44111f75-b0ad-4a42-ac0a-9340d0d71f14")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        [RouteSegment(1)]
        string UrlTitle { get; set; }
        
        [ImmutableFieldId("45bcc9e8-69a7-45a6-a085-36951b4501cc")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        [FieldPosition(2)]
        [StringSizeValidator(0, 512)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, true, false)]
        string Summary { get; set; }
        
        [ImmutableFieldId("c60f92c7-a30c-4faf-a4ca-b88393615e7f")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048, IsNullable=true)]
        [FieldPosition(3)]
        [NullStringLengthValidator(0, 2048)]
        [SearchableField(false, true, false)]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes=true, NullableString=true)]
        string Image { get; set; }
        
        [ImmutableFieldId("42c3637a-4834-4bf9-af42-484cfbe92cf7")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(4)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(IArticleCategory), nameof(IArticleCategory.Id), AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        [SearchableField(false, false, true)]
        Guid Category { get; set; }
        
        [ImmutableFieldId("22cd4ce4-d11e-43e8-81bc-19700edee23c")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [FieldPosition(5)]
        [DefaultFieldStringValue("")]
        [SearchableField(false, false, true)]
        string Tags { get; set; }
        
        [ImmutableFieldId("e2e8832b-b2c3-4c09-b335-cf2c9e68cddf")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
            "ils.Date.Now\" />")]
        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [DateTimeRangeValidator("1753-01-01T00:00:00", "9999-12-31T23:59:59")]
        [FieldPosition(6)]
        [DefaultFieldNowDateTimeValue]
        [RouteDateSegment(0, DateSegmentFormat.YearMonthDay)]
        [SearchableField(false, true, false)]
        [TreeOrdering(1, true)]
        DateTime Date { get; set; }
        
        [ImmutableFieldId("70318a9b-80d5-4373-bd3e-4dd91e119f42")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"" />")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.XhtmlDocument\"><f:param name=\"Constant\"><html xmlns=\"http://www.w3.org/19" +
            "99/xhtml\"><head /><body /></html></f:param></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(7)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, false, false)]
        string Content { get; set; }
    }
}