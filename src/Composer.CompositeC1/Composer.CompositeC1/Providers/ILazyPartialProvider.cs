using Orckestra.Composer.CompositeC1.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public interface ILazyPartialProvider
    {
        string ProtectFunctionCall(string functionName, Dictionary<string, string> parameters);
        LazyFunctionCall UnprotectFunctionCall(string encryptedValue);
    }
}
