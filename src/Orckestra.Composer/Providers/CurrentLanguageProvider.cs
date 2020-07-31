using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orckestra.Composer.Providers
{
    public class ContextLanguageProvider : IContextLanguageProvider
    {
        internal static readonly string HttpContextItem_CurrentCulture = "C1_CurrentCulture";
        public CultureInfo GetCurrentCultureInfo()
        {
            return CallContext.GetData(HttpContextItem_CurrentCulture) as CultureInfo ?? Thread.CurrentThread.CurrentCulture;
        }
    }

    public interface IContextLanguageProvider
    {
        CultureInfo GetCurrentCultureInfo();
    }
}
