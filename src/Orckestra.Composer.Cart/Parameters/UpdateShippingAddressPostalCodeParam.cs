namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateShippingAddressPostalCodeParam : BaseCartParam
    {
        public string BaseUrl { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }
    }
}