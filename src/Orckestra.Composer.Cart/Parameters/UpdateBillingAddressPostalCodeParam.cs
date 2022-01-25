namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateBillingAddressPostalCodeParam : BaseCartParam
    {
        public string BaseUrl { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }
    }
}