namespace Orckestra.Composer.Utils
{
    public static class Constants
    {
        public static class ProductAttributes
        {
            public const string RecurringOrderProgramName = "RecurringOrderProgramName";
        }

        public const string GlobalScopeName = "Global";

        public static class OrderStatus
        {
            public const string Canceled = "Canceled";
            public const string Fulfilled = "Fulfilled";
        }

        public static class OrderDraft
        {
            public const string OwnershipPropertyBagKey = "OrderDraftOwnership";
            public const string OwnershipByWebsite = "oco,orchestrator,website,wfecm,wfecd";
        }

        public static class ErrorCodes
        {
            public const string IsOwnedByRequestedUser = "IsOwnedByRequestedUser";
            public const string IsOwnedBySomeoneElse = "IsOwnedBySomeoneElse";
        }

        public const string DefaultOrderCancellationReason = "Cancelled at Customer request";
    }
}
