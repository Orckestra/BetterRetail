using Composite.Data;
using Orckestra.Composer.Services;
using System;
using System.Linq;

namespace Orckestra.Composer.Recipes.Settings
{
    public class RecipeSettings: IRecipeSettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.IRecipeSettingsMeta RecipeSettingsMeta;

        public RecipeSettings(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
            using (var con = new DataConnection())
            {
                RecipeSettingsMeta = con.Get<DataTypes.IRecipeSettingsMeta>().FirstOrDefault(g => g.PageId == WebsiteContext.WebsiteId);
            }
        }
        public Guid RecipeFavoritesPageId
        {
            get
            {
                if (RecipeSettingsMeta != null && RecipeSettingsMeta.FavoritesPageId.HasValue)
                {
                    return RecipeSettingsMeta.FavoritesPageId.Value;
                }

                return Guid.Empty;
            }
        }
    }
}
