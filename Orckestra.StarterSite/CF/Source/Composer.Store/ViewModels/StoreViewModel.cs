using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreViewModel : BaseViewModel
    {
        /// <summary>
        /// the unique identifier of the Store.,
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the store. Each store must have a name that unique in the system
        /// </summary>
        public string Name { get; set; }

        public string Number { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// The display name of the store. This is a multilingual value.,
        /// </summary>
        public Dictionary<string, string> DisplayName { get; set; }

        public string LocalizedDisplayName { get; set; }

        public StoreInventoryStatusViewModel InventoryStatus { get; set; }

        public StoreAddressViewModel Address { get; set; }

        public StoreScheduleViewModel Schedule { get; set; }

        public string Url { get; set; }

        public Guid FulfillmentLocationId { get; set; }

        public double DestinationToSearchPoint { get; set; }

        public int SearchIndex { get; set; }

        public string GoogleDirectionsLink { get; set; }

        public string GoogleStaticMapUrl { get; set; }
        /// <summary>
        /// local business structured data - see details here - https://developers.google.com/structured-data/local-businesses/
        /// </summary>
        public StoreStructuredDataViewModel StructuredData { get; set; }

        [MapTo("DisplayName")]
        public Dictionary<string, string> LocalizedDisplayNames { get; set; }
    }
}
