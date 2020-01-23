using Newtonsoft.Json;
using Orckestra.Composer.CompositeC1.Api;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class LazyPartialProvider : ILazyPartialProvider
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        private static readonly EncryptionUtility EncryptionUtility = new EncryptionUtility();
        
        public LazyPartialProvider(HttpContextBase httpContext) {
            HttpContext = httpContext;
        }

        public HttpContextBase HttpContext { get; }

        public LazyFunctionCall UnprotectFuntionCall(string encryptedValue)
        {
            try
            {
                var json = EncryptionUtility.Decrypt(encryptedValue);
                var lazyFunctionCall = JsonConvert.DeserializeObject<LazyFunctionCall>(json);
                return lazyFunctionCall;
            }
            catch (Exception ex)
            {
                var errorMessage =
                    "Either someone tampered with his function call or the encryption method changed (machine key changed?) or the Deserialize changed";

                Log.ErrorException(errorMessage, ex);
                return null;
            }

        }

        public string ProtectFunctionCall(string functionName, Dictionary<string, string> parameters)
        {
            var lazyFunctionCall = new LazyFunctionCall()
            {
                FunctionName = functionName,
                Parameters = parameters,
                QueryString = HttpContext.Request.QueryString.ToString()

            };
            string json = JsonConvert.SerializeObject(lazyFunctionCall);

            return EncryptionUtility.Encrypt(json);
        }
    }
}
