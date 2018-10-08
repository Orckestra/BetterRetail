using System;
using Fasterflect;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// This attribute is used to mark an interface as a Metadata provider for a given <see cref="BaseViewModel"/> type.
    /// </summary>
    /// <remarks>
    /// Many Metadata interfaces may be used for a single ViewModel type, but this is not recommended as properties must
    /// be unique.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class ViewModelMetadataAttribute : Attribute
    {
        public Type ViewModelType { get; private set; }

        public ViewModelMetadataAttribute(Type viewModelType)
        {
            if (viewModelType == null) { throw new ArgumentNullException("viewModelType"); }

            if (!viewModelType.Inherits(typeof (BaseViewModel)))
            {
                throw new ArgumentException("Specified type must derive from Orckestra.Composer.ViewModels.BaseViewModel", "viewModelType");
            }

            ViewModelType = viewModelType;
        }
    }
}
