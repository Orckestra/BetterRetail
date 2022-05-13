using System;
using System.Globalization;

namespace Orckestra.Composer.Recipes.Parameters
{
    public class UpdateFavoriteParam
    {
        public string ScopeId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string EntityTypeName { get; set; }
        public string AttributeName { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
