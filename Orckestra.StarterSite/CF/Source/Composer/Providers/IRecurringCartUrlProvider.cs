using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Provider for urls required by the Recurring Orders Concerns
    /// </summary>
    public interface IRecurringCartUrlProvider
    {
        /// <summary>
        /// Url to the Recurring Cart Page
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetRecurringCartsUrl(GetRecurringCartsUrlParam param);
       
        /// <summary>
        /// Url to the Recurring Cart Details Page
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetRecurringCartDetailsUrl(GetRecurringCartDetailsUrlParam param);
    }
}
