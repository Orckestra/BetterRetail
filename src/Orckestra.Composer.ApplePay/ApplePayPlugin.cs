namespace Orckestra.Composer.Cart
{
    public class ApplePayPlugin : IComposerPlugin
    {
        /// <summary>
        /// Autowireup this plugin
        /// </summary>
        /// <param name="host"></param>
        public void Register(IComposerHost host)
        {
            host.RegisterApiControllers(typeof(ApplePayPlugin).Assembly);
        }
    }
}