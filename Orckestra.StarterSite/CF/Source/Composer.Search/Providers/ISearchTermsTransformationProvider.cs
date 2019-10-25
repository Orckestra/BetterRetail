namespace Orckestra.Composer.Search.Providers
{
    public interface ISearchTermsTransformationProvider
    {
        string TransformSearchTerm(string keyword, string cultureInfoName);
    }
}
