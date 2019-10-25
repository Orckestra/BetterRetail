namespace Orckestra.Composer.Search.Providers
{
    public interface ISearchTermsTransformationProvider
    {
        string ExceptionMapOutput(string keyword, string cultureInfoName);
    }
}
