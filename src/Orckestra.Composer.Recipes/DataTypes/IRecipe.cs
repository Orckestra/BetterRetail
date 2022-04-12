using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using Composite.Data.Types;

namespace Orckestra.Composer.Recipes.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(PropertyDataAncestorProvider))]
    [PropertyDataAncestorProvider(nameof(PageId), typeof(IPage), nameof(IPage.Id), null)]
    [ImmutableTypeId("3aa80d36-1935-4a31-9375-5baf50f962dd")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Recipe")]
    [LabelPropertyName("Title")]
    [InternalUrl("Recipes")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    [SearchableType]
    public interface IRecipe : IData, IProcessControlled, IPageDataFolder, IPageRelatedData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("a0b27c28-4a28-477e-a116-df2259df7b2e")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("92f1af7c-7c15-4f41-939a-82cd036acc63")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.ImageSelector""><f:param name=""Required"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 2048)]
        [DefaultFieldStringValue("")]
        [ForeignKey("Composite.Data.Types.IImageFile,Composite", AllowCascadeDeletes=true, NullReferenceValue=null)]
        [SearchableField(false, true, false)]
        string Image { get; set; }
        
        [ImmutableFieldId("0e423861-8910-476f-adc5-73b5c403268f")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, false, false)]
        string Title { get; set; }

        [ImmutableFieldId("bdbc57d5-aae2-4668-90b1-3fe881dae300")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [FieldPosition(1)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, false, false)]
        string Keywords { get; set; }

        [ImmutableFieldId("5d61e9da-0b5e-4e98-8ccc-e1d3f364c429")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [FieldPosition(2)]
        [DefaultFieldStringValue("")]
        [RouteSegment(1)]
        string UrlTitle { get; set; }
        
        [ImmutableFieldId("204b7a2e-36a6-4a79-bcef-006d9ffcd04c")]
        [FormRenderingProfile(Label = "Meal Type")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(3)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(IRecipeMealType), nameof(IRecipeMealType.Id), AllowCascadeDeletes=true, NullReferenceValue="{00000000-0000-0000-0000-000000000000}")]
        [SearchableField(false, false, true)]
        Guid MealType { get; set; }

        [ImmutableFieldId("5c10bf13-1b2a-48b7-ad6a-f1ea14c9ee9f")]
        [FormRenderingProfile(Label = "Dish Type")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(8)]
        [ForeignKey(typeof(IDishType), nameof(IDishType.Id), AllowCascadeDeletes = true)]
        [SearchableField(false, false, true)]
        Nullable<Guid> DishType { get; set; }

        [ImmutableFieldId("0b26f753-dbfc-4292-a262-0a4efeb0bdc5")]
        [FormRenderingProfile(Label = "Diet Type", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.DataIdMultiSelector""><f:param name=""OptionsType"" value=""Orckestra.Composer.Recipes.DataTypes.IDietType"" /><f:param name=""CompactMode"" value=""False"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [FieldPosition(9)]
        [NullStringLengthValidator(0, 256)]
        [SearchableField(false, true, true)]
        string DietType { get; set; }

        [ImmutableFieldId("9065ffb5-3a8c-43d7-988b-7ee4c97177f8")]
        [FormRenderingProfile(Label = "Cuisine Type")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [FieldPosition(10)]
        [ForeignKey(typeof(ICuisineType), nameof(ICuisineType.Id), AllowCascadeDeletes = true)]
        [SearchableField(false, false, true)]
        Nullable<Guid> CuisineType { get; set; }

        [ImmutableFieldId("5dcfd032-0c8d-4884-b403-dcfee5411fb7")]
        [FormRenderingProfile(Label = "Preparation Time ", HelpText = "Preparation time in minutes")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(11)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        [SearchableField(false, true, false)]
        Nullable<int> PreparationTime { get; set; }

        [ImmutableFieldId("65dfb3cf-a93c-4448-a926-73ed70a2cdb9")]
        [FormRenderingProfile(Label = "Cooking Time", HelpText = "Cooking time in minutes")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(12)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        [SearchableField(false, true, false)]
        Nullable<int> CookingTime { get; set; }

        [ImmutableFieldId("31164ebb-983a-4f8c-9fd4-261156c48b65")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        [FieldPosition(13)]
        [SearchableField(false, true, false)]
        string Servings { get; set; }

        [ImmutableFieldId("d830604a-a591-4e64-871e-b8235b46f0ac")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"" />")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.XhtmlDocument\"><f:param name=\"Constant\"><html xmlns=\"http://www.w3.org/19" +
            "99/xhtml\"><head /><body /></html></f:param></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(4)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, false, false)]
        string Description { get; set; }
        
        [ImmutableFieldId("2045b3d6-c741-4f03-9804-65c39bd9b09d")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"" />")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.XhtmlDocument\"><f:param name=\"Constant\"><html xmlns=\"http://www.w3.org/19" +
            "99/xhtml\"><head /><body /></html></f:param></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [NotNullValidator]
        [FieldPosition(5)]
        [DefaultFieldStringValue("")]
        [SearchableField(true, false, false)]
        string Instructions { get; set; }

        [ImmutableFieldId("85a1a2fc-8bb8-4440-ac5e-6fd59618e50d")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [GuidNotEmpty]
        [FieldPosition(6)]
        [DefaultFieldGuidValue("00000000-0000-0000-0000-000000000000")]
        [ForeignKey(typeof(IDifficulty), nameof(IDifficulty.Id), AllowCascadeDeletes = true, NullReferenceValue = "{00000000-0000-0000-0000-000000000000}")]
        [SearchableField(false, true, true)]
        Guid Difficulty { get; set; }

        [ImmutableFieldId("503433f6-e144-49d4-a01f-b0f6b7952b85")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
            "ils.Date.Now\" />")]
        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [DateTimeRangeValidator("1753-01-01T00:00:00", "9999-12-31T23:59:59")]
        [FieldPosition(7)]
        [DefaultFieldNowDateTimeValue]
        [TreeOrdering(1, true)]
        DateTime Date { get; set; }
    }
}