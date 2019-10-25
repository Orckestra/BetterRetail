using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Orckestra.Composer.Articles.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("7bcae902-493d-4edd-93f9-b7f437793ed9")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("ArticlesCategories")]
    [LabelPropertyName("Title")]
    [Caching(CachingType.Full)]
    [SearchableType]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    public interface IArticleCategory : IData, IPublishControlled, ILocalizedControlled
    {
        [ImmutableFieldId("1a3703ad-fb2f-4660-8f6f-fddab1ae1cb5")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(0)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("afb5b380-95db-487b-a762-4a36dd579be6")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator]
        [FieldPosition(0)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string Title { get; set; }
        
        [ImmutableFieldId("38596154-1fa0-4951-a044-08d7c5476103")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator]
        [FieldPosition(1)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        [RouteSegment(0)]
        string UrlTitle { get; set; }
    }
}