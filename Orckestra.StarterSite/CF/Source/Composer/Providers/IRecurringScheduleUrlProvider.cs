using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Provider for urls required by the Recurring Orders Concerns
    /// </summary>
    public interface IRecurringScheduleUrlProvider
    {
        /// <summary>
        /// Url to the Recurring Schedule Page
        /// </summary>
        /// <returns>localized url</returns>
        string GetRecurringScheduleUrl(GetRecurringScheduleUrlParam param);

        /// <summary>
        /// Base Url to the Recurring Schedule Details Page
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetRecurringScheduleDetailsBaseUrl(GetRecurringScheduleDetailsBaseUrlParam param);

        /// <summary>
        /// Url to the Recurring Schedule Details Page
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetRecurringScheduleDetailsUrl(GetRecurringScheduleDetailsUrlParam param);
    }
}
