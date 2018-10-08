using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class InventoryQuantityViewModel : BaseViewModel
    {
        /// <summary>
        /// The inventory quantity identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The inventory location identifier
        /// </summary>
        public string InventoryLocationId { get; set; }

        /// <summary>
        /// Tthe sku of the product
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// The value indicating whether this instance is back orderable
        /// </summary>
        public bool IsBackOrderable { get; set; }

        /// <summary>
        /// The reserved quantity for back order
        /// </summary>
        public double? BackOrderReservedQuantity { get; set; }

        /// <summary>
        /// The maximum number of product that can be taken from the inventory in back order
        /// </summary>
        public double? BackOrderLimit { get; set; }

        /// <summary>
        /// The value indicating whether this instance is pre orderable
        /// </summary>
        public bool IsPreOrderable { get; set; }

        /// <summary>
        /// The reserved quantity for pre order
        /// </summary>
        public double? PreOrderReservedQuantity { get; set; }

        /// <summary>
        /// The maximum number of product that can be taken from the inventory
        /// </summary>
        public double? PreOrderLimit { get; set; }

        /// <summary>
        /// The quantity of this item
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// The reserved quantity of this item
        /// </summary>
        public double ReservedQuantity { get; set; }

        /// <summary>
        /// The number of items which should trigger an alert to re-order the item
        /// </summary>
        public double? ReOrderPoint { get; set; }
    }
}
