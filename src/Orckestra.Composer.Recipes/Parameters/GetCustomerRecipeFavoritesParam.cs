using System;
using System.Globalization;

namespace Orckestra.Composer.Recipes.Parameters
{
    public class GetCustomerRecipeFavoritesParam
    {
        public string Scope { get; set; }
        public Guid CustomerId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
