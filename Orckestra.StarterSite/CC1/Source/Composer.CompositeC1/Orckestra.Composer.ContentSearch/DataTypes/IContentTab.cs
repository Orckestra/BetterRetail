using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.ContentSearch.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("89480158-9f1f-4a4a-8743-49071f38aa16")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Content Search Tab")]
    [LabelPropertyName("Title")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    [Caching(CachingType.Full)]
    public interface IContentTab : IData, IProcessControlled, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("1cc857f9-39d2-4d2a-8453-01e835eea985")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        [RouteSegment(0)]
        Guid Id { get; set; }

        [ImmutableFieldId("4842ea4c-2829-4571-a8da-5b640dabb06b")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }

        [ImmutableFieldId("7f27a124-6d50-4ecc-b7f9-4de17b60ac43")]
        [FormRenderingProfile(Label = "Url Title")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        string UrlTitle { get; set; }

        [ImmutableFieldId("7c55195a-3412-4add-9d79-065ffa3f46db")]
        [FormRenderingProfile(Label = "Data Types", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Search.WebsiteSearch.WebsiteSearchFacade.GetSearchableDataTypeOptions"" /></f:param><f:param name=""KeyFieldName"" value=""Item1"" /><f:param name=""LabelFieldName"" value=""Item2"" /><f:param name=""Multiple"" value=""True"" /><f:param name=""Compact"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [FieldPosition(2)]
        string DataTypes { get; set; }

        [ImmutableFieldId("0a52e9a7-bf9d-4ea3-bbbe-e81f01fb5189")]
        [FormRenderingProfile(WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.Selector""><f:param xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Options""><f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Orckestra.Search.WebsiteSearch.WebsiteSearchFacade.GetFacetOptions"" /></f:param><f:param name=""KeyFieldName"" value=""Item1"" /><f:param name=""LabelFieldName"" value=""Item2"" /><f:param name=""Multiple"" value=""True"" /><f:param name=""Compact"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [FieldPosition(3)]
        string Facets { get; set; }

        [ImmutableFieldId("a7325a27-6354-4363-b5c6-0e4974add6c7")]
        [FormRenderingProfile(Label = "Media Folders", HelpText = "Select specific media folders to make search by media files for current website only", WidgetFunctionMarkup = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Widgets.String.DataIdMultiSelector""><f:param name=""OptionsType"" value=""Composite.Data.Types.IMediaFileFolder,Composite"" /><f:param name=""CompactMode"" value=""True"" /></f:widgetfunction>")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [FieldPosition(4)]
        string MediaFolders { get; set; }

        [ImmutableFieldId("96d4d695-b6b5-4619-aee6-ca3893caabe0")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(5)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        [TreeOrdering(1)]
        Nullable<int> Order { get; set; }

        [ImmutableFieldId("7b32e60c-27c6-491b-af34-e02d0697c17a")]
        [FormRenderingProfile(Label = "Is Product Tab")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(1)]
        [DefaultFieldBoolValue(false)]
        bool IsProductTab { get; set; }
    }
}