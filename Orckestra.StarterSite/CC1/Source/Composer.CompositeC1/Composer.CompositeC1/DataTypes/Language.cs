using System;
using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Validation.Validators;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [AutoUpdateble]
    //[DataScope("public")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataScope(DataScopeIdentifier.AdministratedName)]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("edfe60c8-5405-4313-b138-18549454319a")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("Language")]
    [LabelPropertyName("DisplayName")]
    [PublishProcessControllerType(typeof(GenericPublishProcessController))]
    public interface Language : IData, IProcessControlled, IPublishControlled
    {
        [ImmutableFieldId("d81a3c38-7a98-4c69-90d7-8a54bb473590")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [FieldPosition(-1)]
        Guid Id { get; set; }
        
        [ImmutableFieldId("4ee59c3b-43fe-4fa0-a1ca-0d5d72e5a386")]
        [StoreFieldType(PhysicalStoreFieldType.String, 16)]
        //[NotNullValidator]
        [FieldPosition(0)]
        //[StringSizeValidator(0, 16)]
        [DefaultFieldStringValue("")]
        string Code { get; set; }
        
        [ImmutableFieldId("36596aa9-c4ee-460a-a289-b866884413d6")]
        [FormRenderingProfile(Label = "Is Active")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [FieldPosition(1)]
        [DefaultFieldBoolValue(false)]
        bool IsActive { get; set; }
        
        [ImmutableFieldId("9a2e8b4f-ada6-483f-adf2-c25c2f4f0a40")]
        [FormRenderingProfile(Label = "Display Name")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        //[NotNullValidator]
        [FieldPosition(2)]
        //[StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string DisplayName { get; set; }

        [ImmutableFieldId("9a2e8b4f-ada6-483f-adf2-c25c2f4f0a41")]
        [FormRenderingProfile(Label = "Short Display Name")]
        [StoreFieldType(PhysicalStoreFieldType.String, 32)]
        //[NotNullValidator]
        [FieldPosition(3)]
        //[StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        string ShortDisplayName { get; set; }
    }
}