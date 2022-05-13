using System.Collections.Generic;

namespace Orckestra.Composer.Recipes.Requests
{
    public class SearchRecipesRequest
    {
        public string QueryString { get; set; }
        public List<string> FavoriteIds { get; set; }
    }
}
