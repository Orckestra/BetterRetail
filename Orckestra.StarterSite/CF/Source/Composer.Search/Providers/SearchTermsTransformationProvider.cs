namespace Orckestra.Composer.Search.Providers
{
    /// <summary>
    /// A list of multi word exceptions that should have their spaces replaced with dashes("-")
    /// </summary>
    public class SearchTermsTransformationProvider : ISearchTermsTransformationProvider
    {
        public virtual string TransformSearchTerm(string keyword, string cultureInfoName)
        {
            return keyword;
        }
    }
}
