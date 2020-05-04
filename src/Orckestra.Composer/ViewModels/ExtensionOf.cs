using System;
using Orckestra.Composer.Providers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    ///     Abstract class dynamically implemented by proxy classes extending view models of type <typeparamref name="TModel" />.
    ///     The proxy classes are created by the <see cref="ExtensionTypeProxyFactory" />.
    /// </summary>
    /// <typeparam name="TModel">Type of view model extended by this class. It extends from <see cref="IBaseViewModel" /></typeparam>
    public abstract class ExtensionOf<TModel> : IExtension, IExtensionOf<TModel>
        where TModel : class, IBaseViewModel
    {
        /// <summary>
        ///     Sets the extended entity
        /// </summary>
        public void SetBaseViewModel(IBaseViewModel baseViewModel)
        {
            if (baseViewModel == null) { throw new ArgumentNullException(nameof(baseViewModel)); }

            Model = (TModel)baseViewModel;
        }

        /// <summary>
        ///     Gets or sets the extended entity
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        ///     Getter method used by all properties of this class to access the value from the property bag of the extended
        ///     entity.
        /// </summary>
        protected T GetValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(propertyName)); }

            if (Model == null || Model.Bag == null || !Model.Bag.TryGetValue(propertyName, out object value) || value == null)
            {
                return default;
            }
            return (T)value;
        }

        /// <summary>
        ///     Getter method used by all properties of this class to set the value to the property bag of the extended entity.
        /// </summary>
        protected void SetValue<T>(T value, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(propertyName)); }

            if (Model == null || Model.Bag == null)
            {
                return;
            }
            Model.Bag[propertyName] = value;
        }
    }
}