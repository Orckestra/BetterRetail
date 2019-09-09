using Orckestra.Composer.Parameters;
using System;
using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoresDirectoryUrlParam: BaseUrlParameter
    {
        public string BaseUrl { get; set; }
        public int Page { get; set; }
    }
}
