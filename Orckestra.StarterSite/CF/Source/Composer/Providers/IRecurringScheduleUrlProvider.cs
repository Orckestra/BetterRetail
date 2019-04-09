using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Provider for urls required by the Recurring Orders Concerns
    /// </summary>
    public interface IRecurringScheduleUrlProvider
    {
        /// <summary>
        /// Url to the My Account page
        /// </summary>
        /// <returns>localized url</returns>
        string GetRecurringScheduleUrl(GetRecurringScheduleUrlParam param);        
    }
}
