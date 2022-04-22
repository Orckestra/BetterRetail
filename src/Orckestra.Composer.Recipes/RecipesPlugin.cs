namespace Orckestra.Composer.Recipes
{
    public class RecipesPlugin: IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.RegisterApiControllers(typeof(RecipesPlugin).Assembly);
        }
    }
}
