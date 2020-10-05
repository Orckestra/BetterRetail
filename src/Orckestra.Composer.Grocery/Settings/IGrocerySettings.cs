using System;

namespace Orckestra.Composer.Grocery.Settings
{
    public interface IGrocerySettings
    {
        string DefaultStore { get; }

        /// <summary>
        /// For how many days ahead (including current one) time slot selection should be shown.
        /// </summary>
        int TimeSlotsDaysAmount { get; }
        /// <summary>
        /// Time span after which a reservation is considered expired.
        /// </summary>
        TimeSpan ReservationExpirationTimeSpan { get; }
        /// <summary>
        /// Time span after which a message about a reservation's expiration time should be shown to a customer.
        /// </summary>
        TimeSpan ReservationExpirationWarningTimeSpan { get; }
    }
}