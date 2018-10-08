namespace Orckestra.Composer.ViewModels
{
    public interface IExtensionOf<TModel>
    {
        TModel Model { get; set; }
    }

    /// <summary>
    /// Interface only used by reflection to dynamically create proxy extension types of view models
    /// </summary>
    internal interface IExtension
    {
        /// <summary>
        /// Sets the extended entity
        /// </summary>
        void SetBaseViewModel(IBaseViewModel baseViewModel);
    }
}