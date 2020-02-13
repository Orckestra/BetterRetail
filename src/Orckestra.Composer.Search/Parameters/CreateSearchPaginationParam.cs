using Orckestra.Composer.Search.Services;

namespace Orckestra.Composer.Search.Parameters
{
    public class CreateSearchPaginationParam<TSearchParam> 
        where TSearchParam: class, ISearchParam
    {
        public TSearchParam SearchParameters { get; set; }

        public int CurrentPageIndex { get; set; }

        public int TotalNumberOfPages { get; set; }

        public int MaximumPages { get; set; }

        public string CorrectedSearchTerms { get; set; }
    }
}
