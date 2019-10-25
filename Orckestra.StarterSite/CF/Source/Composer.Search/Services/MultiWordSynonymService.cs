namespace Orckestra.Composer.Search.Services
{
	//A list of multi word exceptions that should have their spaces replaced with dashes("-")

    public class MultiWordSynonymService : IMultiWordSynonymService
    {
        public virtual string ExceptionMapOutput(string keyword, string cultureInfoName)
        {
            return keyword;
        }
    }
}
