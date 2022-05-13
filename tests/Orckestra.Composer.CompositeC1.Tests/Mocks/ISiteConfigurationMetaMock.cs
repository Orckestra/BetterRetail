using System;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.ExperienceManagement.Configuration.DataTypes;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class ISiteConfigurationMetaMock : ISiteConfigurationMeta
    {
        public string Scope { get; set; }
        public string CountryCode { get; set; }
        public string InventoryAndFulfillmentLocationId { get; set; }
        public Guid? ProductDetailsPage { get; set; }
        public Guid? LoginPage { get; set; }
        public Guid? MyAccountPage { get; set; }
        public Guid? CreateAccountPageId { get; set; }
        public Guid? ForgotPasswordPageId { get; set; }
        public Guid? ResetPasswordPageId { get; set; }
        public Guid? ChangePasswordPageId { get; set; }
        public Guid? TermsAndConditionsPageId { get; set; }
        public Guid? SearchPageId { get; set; }
        public Guid? CartPageId { get; set; }
        public Guid? StoreListPageId { get; set; }
        public Guid? StorePageId { get; set; }
        public Guid? StoreDirectoryPageId { get; set; }
        public Guid? CheckoutAddAddressPageId { get; set; }
        public Guid? CheckoutUpdateAddressPageId { get; set; }
        public Guid? MyWishListPageId { get; set; }
        public Guid? SharedWishListPageId { get; set; }
        public Guid? PageNotFoundPageId { get; set; }
        public Guid? CheckoutSignInPageId { get; set; }
        public Guid? AddressListPageId { get; set; }
        public Guid? AddAddressPageId { get; set; }
        public Guid? UpdateAddressPageId { get; set; }
        public Guid? OrderHistoryPageId { get; set; }
        public Guid? OrderDetailsPageId { get; set; }
        public Guid? GuestOrderDetailsPageId { get; set; }
        public Guid? FindMyOrderPageId { get; set; }
        public string CreditCardsTrustIconId { get; set; }
        public string CheckoutSteps { get; set; }
        public Guid? CheckoutPageId { get; set; }
        public Guid? CheckoutConfirmationPageId { get; set; }
        public string CheckoutNavigation { get; set; }
        public string FieldName { get; set; }
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public string PublicationStatus { get; set; }
        public Guid VersionId { get; set; }
        public string SourceCultureName { get; set; }

        public DataSourceId DataSourceId => throw new NotImplementedException();
    }
}
