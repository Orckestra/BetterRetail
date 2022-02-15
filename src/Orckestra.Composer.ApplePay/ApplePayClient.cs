using Composite.Core;
using Newtonsoft.Json;
using Orckestra.Composer.ApplePay.Requests;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ApplePay
{
    public class ApplePayClient
    {
        private readonly HttpClient _httpClient;
        private readonly MerchantCertificate _certificate;

        public ApplePayClient()
        {
            _certificate = new MerchantCertificate(new Options.ApplePayOptions());
            var certificate = _certificate.GetCertificate();

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };
            handler.ClientCertificates.Add(certificate);
            _httpClient = new HttpClient(handler);
        }

        public async Task<object> GetMerchantSessionAsync(Uri requestUri, ApplePaySessionRequest request)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // POST the data to create a valid Apple Pay merchant session.
            string json = JsonConvert.SerializeObject(request);

            try
            {

                var content = new StringContent(json, Encoding.UTF8, "application/json");
              
                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();
                // Read the opaque merchant session JSON from the response body.
                var stream = await response.Content.ReadAsStreamAsync();
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize(jsonTextReader);
                }
            }
            catch(Exception ex)
            {
                Log.LogError("Appl Pay", ex);
                return ex;
            }
        }
    }
}
