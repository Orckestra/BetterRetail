namespace Orckestra.Composer.ApplePay.Options
{
    public class ApplePayOptions
    {
        public string StoreName => "";

        public bool UseCertificateStore => false; // optional

        public string MerchantCertificate => ""; // optional

        public string MerchantCertificateFileName => "certificate.pfx";

        public string MerchantCertificatePassword => "testpass";

        public string MerchantCertificateThumbprint => ""; // optional
    }
}