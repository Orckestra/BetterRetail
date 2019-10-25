namespace Orckestra.Composer.Search.Providers
{
	//A list of multi word exceptions that should have their spaces replaced with dashes("-")

    public class SearchTermsTransformationProvider : ISearchTermsTransformationProvider
    {
        public virtual string TransformSearchTerm(string keyword, string cultureInfoName)
        {
            return keyword;
        }
    }
}
