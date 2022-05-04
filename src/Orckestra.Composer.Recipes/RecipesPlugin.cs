using Orckestra.Composer.Recipes.Settings;
using Orckestra.Overture;

namespace Orckestra.Composer.Recipes
{
    public class RecipesPlugin: IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<RecipeSettings, IRecipeSettings>(ComponentLifestyle.PerRequest);
            host.RegisterApiControllers(typeof(RecipesPlugin).Assembly);
        }
    }
}
