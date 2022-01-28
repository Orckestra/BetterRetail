namespace Orckestra.Composer.Utils
{
    public static class Constants
    {
        public static class ProductAttributes
        {
            public const string RecurringOrderProgramName = "RecurringOrderProgramName";
        }

        public const string GlobalScopeName = "Global";

        public static class OrderDraft
        {
            public const string OrderNumberProbertyBagKey = "OrderNumberForOrderDraft";
            public const string OwnershipPropertyBagKey = "OrderDraftOwnership";
            public const string OwnershipByWebsite = "oco";
        }

        public static class ErrorCodes
        {
            public const string IsOwnedByRequestedUser = "IsOwnedByRequestedUser";
            public const string IsOwnedBySomeoneElse = "IsOwnedBySomeoneElse";
        }
    }
}
