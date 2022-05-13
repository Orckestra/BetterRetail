using System;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Services
{
    public interface ISearchParam : ICloneable
    {
        SearchCriteria Criteria { get; set; }
    }
}
