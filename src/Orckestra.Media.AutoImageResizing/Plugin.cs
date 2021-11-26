using Composite.Core.WebClient.Renderings.Page;
using Orckestra.Composer;

namespace Orckestra.Media.AutoImageResizing
{
    public class Plugin : IComposerPlugin
    {
        /// <summary>
        /// Autowireup this plugin
        /// </summary>
        /// <param name="host"></param>
        public void Register(IComposerHost host)
        {
            host.Register<ImageResizer, IPageContentFilter>();
        }
    }
}
