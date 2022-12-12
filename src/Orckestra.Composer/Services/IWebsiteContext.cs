using System;
using Composite.Data;

namespace Orckestra.Composer.Services
{
    public interface IWebsiteContext
    {
        /// <summary>
        /// The ID of the root page of the current website.
        /// </summary>
        Guid WebsiteId { get; }

        /// <summary>
        /// Gets the meta data attached to the root page of the current website.
        /// </summary>
        T GetRootPageMetaData<T>() where T : class, IPageMetaData;
    }
}