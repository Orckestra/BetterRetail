using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public interface IWebsiteContext
    {
        Guid WebsiteId { get; }
    }
}
