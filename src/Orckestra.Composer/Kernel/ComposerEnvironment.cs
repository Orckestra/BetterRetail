using System.Web;
using System.Web.Hosting;

namespace Orckestra.Composer.Kernel
{
    /// <summary>
    /// This object contains environment variables.
    /// </summary>
    internal class ComposerEnvironment : IComposerEnvironment
    {
        /// <summary>
        /// Gets the application domain application path.
        /// </summary>
        /// <value>
        /// The application domain application path.
        /// </value>
        public string AppDomainAppPath
        {
            get { return HttpRuntime.AppDomainAppPath; }
        }

        /// <summary>
        /// Gets the VirtualPathProvider for resolving path on this server
        /// </summary>
        public VirtualPathProvider VirtualPathProvider
        {
            get { return HostingEnvironment.VirtualPathProvider; }
        }
    }
}
