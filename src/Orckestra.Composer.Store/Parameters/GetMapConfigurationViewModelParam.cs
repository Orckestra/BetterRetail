﻿using System.Globalization;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetMapConfigurationViewModelParam
    {
        public string Scope { get; set; }
        public bool LoadStoresBounds { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
