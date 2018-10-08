using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Utils
{
    public class CultureInfoEqualityComparer : IEqualityComparer<CultureInfo>
    {
        public bool Equals(CultureInfo x, CultureInfo y)
        {
            if ((x == null && y == null)) { return true; }
            if (x == null || y == null) { return false; }

            return string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(CultureInfo obj)
        {
            return 0;   //Forcing the call to Equals to occur.
        }
    }
}
