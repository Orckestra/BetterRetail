namespace Orckestra.Composer.ApplePay.Options
{
    public class ApplePayOptions
    {
        public string StoreName => "";

        public bool UseCertificateStore => false; // optional

        public string MerchantCertificate => ""; // optional

        public string MerchantCertificateFileName => "merchant_id.cer";

        public string MerchantCertificatePassword => "1ZAsse4+applePay'Z@";

        public string MerchantCertificateThumbprint => ""; // optional
    }
}