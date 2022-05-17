using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Data.Validation.Validators;
using System;

namespace Orckestra.Composer.Grocery.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("a7c3396e-b421-413c-96dd-abc8571283e6")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("My Usuals Settings")]
    [LabelPropertyName("MyUsualsPage")]
    public interface IMyUsualsSettingsMeta : IData, IProcessControlled, IPageRelatedData, IVersioned, IPublishControlled, ILocalizedControlled, IPageData, IPageMetaData
    {
        [ImmutableFieldId("11da5ef2-f4d9-47ef-863c-991f341ba520")]
        [FormRenderingProfile(Label = "My Usuals Page")]
        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable=true)]
        [FieldPosition(0)]
        [ForeignKey("Composite.Data.Types.IPage,Composite", AllowCascadeDeletes=true)]
        Nullable<Guid> MyUsualsPage { get; set; }
        
        [ImmutableFieldId("2f18a475-2a83-4408-bfc6-8f4818b0b3ba")]
        [FormRenderingProfile(Label = "Time Frame", HelpText = "The time frame for considering frequently purchased products.")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"90\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [LazyFunctionProviedProperty("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
            "ils.Validation.IntegerRangeValidation\">\r\n  <f:param name=\"min\" value=\"1\" />\r\n  <" +
            "f:param name=\"max\" value=\"365\" />\r\n</f:function>")]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        Nullable<int> TimeFrame { get; set; }
        
        [ImmutableFieldId("3400f14e-24de-4913-83c9-72738be9e97a")]
        [FormRenderingProfile(Label = "Frequency", HelpText = "The number of times a product was purchased.")]
        [LazyFunctionProviedProperty("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
            "ils.Validation.IntegerRangeValidation\">\r\n  <f:param name=\"min\" value=\"1\" />\r\n  <" +
            "f:param name=\"max\" value=\"1000\" />\r\n</f:function>")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Co" +
            "nstant.Integer\"><f:param name=\"Constant\" value=\"3\" /></f:function>")]
        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [FieldPosition(2)]
        [NullIntegerRangeValidator(-2147483648, 2147483647)]
        Nullable<int> Frequency { get; set; }
    }
}