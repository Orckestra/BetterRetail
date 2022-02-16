namespace Orckestra.Composer.BamboraPayment
{
    public class BamboraPlugin : IComposerPlugin
    {
        /// <summary>
        /// Autowireup this plugin
        /// </summary>
        /// <param name="host"></param>
        public void Register(IComposerHost host)
        {
            host.RegisterApiControllers(typeof(BamboraPlugin).Assembly);
        }
    }
}