namespace Orckestra.Composer.Providers
{
    public interface IRegionCodeProvider
    {
        string GetRegion(string code, string countryCode);
    }
}