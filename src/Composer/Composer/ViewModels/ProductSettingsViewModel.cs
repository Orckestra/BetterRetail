namespace Orckestra.Composer.ViewModels
{
    public sealed class ProductSettingsViewModel : BaseViewModel
    {
        ///// <summary>
        ///// the number of days the product stays in the New count.
        ///// </summary>
        //public int NewProductDays { get; set; }

        ///// <summary>
        ///// a value indicating whether the product workflow is simple (false) or with composer (true).
        ///// </summary>
        //public bool EnableExtendedProductWorkflow { get; set; }

        ///// <summary>
        ///// System setting to allow overriding the default behavior of not allowing any product with regular prices to be saved at 0 pricing.
        ///// </summary>
        //public bool OverrideGreaterThanZeroPricingConstraint { get; set; }

        ///// <summary>
        ///// whether or not the changes to multilingual attributes in a sales scope should be copied over to the Global scope.
        ///// </summary>
        //public bool AllowTranslationFromSalesToGlobal { get; set; }

        ///// <summary>
        ///// Specify what will be the Active value for the products imported.
        ///// </summary>
        //public ImportProductStatusOption DefaultProductImportStatus { get; set; }

        ///// <summary>
        ///// Whether or not to create notifications (for changes in system culture)  to all other languages of the same scope as the changes.
        ///// </summary>
        //public bool NotifySameScopeOtherLanguages { get; set; }

        ///// <summary>
        ///// Specifies the product and variant sku uniqueness level.
        ///// </summary>
        //public SkuUniquenessLevel SkuUniquenessLevel { get; set; }

        /// <summary>
        /// Determine if the inventory is enabled
        /// </summary>
        public bool IsInventoryEnabled { get; set; }

        ///// <summary>
        ///// the list of products inventory statuses available in search.
        ///// </summary>
        //public string AvailableInventoryStatuses { get; set; }

        ///// <summary>
        ///// the maximum number of variant attributes.
        ///// </summary>
        //public int MaxNumberOfVariantAttributes { get; set; }

        ///// <summary>
        ///// The property bag containing extended properties for this command.
        ///// </summary>
        //public PropertyBag PropertyBag { get; set; }
    }
}
