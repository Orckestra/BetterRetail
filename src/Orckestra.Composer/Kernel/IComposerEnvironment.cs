using System.Web.Hosting;

namespace Orckestra.Composer.Kernel
{
    /// <summary>
    /// Environment variables mapping.
    /// </summary>
    internal interface IComposerEnvironment
    {
        /// <summary>
        /// Gets the application domain application path.
        /// </summary>
        /// <value>
        /// The application domain application path.
        /// </value>
        string AppDomainAppPath { get; }

        /// <summary>
        /// Gets the VirtualPathProvider for resolving path on this server
        /// </summary>
        VirtualPathProvider VirtualPathProvider { get; }
    }
}
