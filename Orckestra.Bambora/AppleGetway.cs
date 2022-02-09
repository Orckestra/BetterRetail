using Bambora.NA.SDK;
using Bambora.NA.SDK.Data;
using Orckestra.Bambora.API;

namespace Orckestra.Bambora
{
    public class AppleGateway
    {
        /// <summary>
        /// The Bambora merchant ID 
        /// </summary>
        public int MerchantId { get; set; }

        /// <summary>
        /// The Apple  merchant ID 
        /// </summary>
        public string AppleMerchantId { get; set; }

        /// <summary>
        /// The API Key (Passcode) for accessing the payments API.
        /// </summary>
        public string PaymentsApiKey
        {
            set
            {
                Configuration.PaymentsApiPasscode = value;
            }
            get
            {
                return Configuration.PaymentsApiPasscode;
            }
        }

        /// <summary>
        /// The api version to use
        /// </summary>
        public string ApiVersion { get; set; }


        private Configuration _configuration { get; set; }

        public Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Configuration();
                    _configuration.MerchantId = this.MerchantId;
                    _configuration.PaymentsApiPasscode = PaymentsApiKey;
                    _configuration.Version = ApiVersion;
                }
                return _configuration;
            }
        }

        public IWebCommandExecuter WebCommandExecuter { get; set; }

        private PaymentsApplePayAPI _payments;

        public PaymentsApplePayAPI Payments
        {
            get
            {
                if (_payments == null)
                    _payments = new PaymentsApplePayAPI();
                _payments.Configuration = Configuration;
                if (WebCommandExecuter != null)
                    _payments.WebCommandExecuter = WebCommandExecuter;
                return _payments;
            }
        }


        public static void ThrowIfNullArgument(object value, string name)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(name);
            }
        }
    }

}
