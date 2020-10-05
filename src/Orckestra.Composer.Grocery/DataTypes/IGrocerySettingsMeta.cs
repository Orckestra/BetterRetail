using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;
using Composite.Data.Validation.Validators;

namespace Orckestra.Composer.Grocery.DataTypes
{
    [AutoUpdateble]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("4f16a00c-b156-45b1-86db-cdfca8ca27b4")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [Title("Grocery Settings")]
    [LabelPropertyName("DefaultStore")]
    public interface IGrocerySettingsMeta : IPageMetaData, ILocalizedControlled
    {
        [ImmutableFieldId("88dc36b1-9c7a-4ccb-94a2-a4c0b6521347")]
        [FormRenderingProfile(Label = "Default Store", HelpText = "Set up the default store number here")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64, IsNullable = true)]
        [FieldPosition(0)]
        [NullStringLengthValidator(0, 64)]
        string DefaultStore { get; set; }

        [ImmutableFieldId("911ce481-69d2-41d1-ab4f-dedbe790c6b0")]
        [FunctionBasedNewInstanceDefaultFieldValue(@"<f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Constant.Integer"">
        <f:param name=""Constant"" value=""7"" /></f:function>")]
        [FormRenderingProfile(Label = "The Number of Days for Time Slots")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(1, 14)]
        int? TimeSlotsDaysAmount { get; set; }

        [ImmutableFieldId("1ba0e075-7b57-48dc-a24c-db3f9d2705b7")]
        [FunctionBasedNewInstanceDefaultFieldValue(@"<f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Constant.Integer"">
        <f:param name=""Constant"" value=""120"" /></f:function>")]
        [FormRenderingProfile(Label = "TimeSlot Reservation Expiration Time", HelpText = "TimeSlot Reservation Expiration Time in minutes")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(1, 1440)]
        int? ReservationExpirationTime { get; set; }

        [ImmutableFieldId("31d3ed7c-d609-4216-9afc-e90c2d77e53a")]
        [FunctionBasedNewInstanceDefaultFieldValue(@"<f:function xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""Composite.Constant.Integer"">
        <f:param name=""Constant"" value=""60"" /></f:function>")]
        [FormRenderingProfile(Label = "The TimeSlot Reservation Expiration Warning Time", HelpText = "TimeSlot Reservation Expiration Warning Time in minutes")]
        [StoreFieldType(PhysicalStoreFieldType.Integer, IsNullable = true)]
        [FieldPosition(1)]
        [NullIntegerRangeValidator(1, 1440)]
        int? ReservationExpirationWarningTime { get; set; }

    }
}
