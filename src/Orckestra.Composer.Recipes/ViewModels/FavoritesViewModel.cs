using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Recipes.ViewModels
{
    public class FavoritesViewModel
    {
        public List<Guid> FavoriteIds { get; set; }
        public string SignInUrl { get; set; }

        public FavoritesViewModel()
        {
            FavoriteIds = new List<Guid>();
        }
    }
}
