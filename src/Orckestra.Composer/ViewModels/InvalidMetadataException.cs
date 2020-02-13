using System;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Exception thrown at metadata registration.
    /// </summary>
    public class InvalidMetadataException : Exception
    {
        /// <summary>
        /// Type targeted by the metadata class.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Type where the metadata is declared.
        /// </summary>
        public Type MetadataType { get; private set; }

        public InvalidMetadataException(Type targetType, Type metadataType, string message)
            : base(message)
        {
            TargetType = targetType;
            MetadataType = metadataType;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        public override string ToString()
        {
            var value =
                string.Format(
                    "Error while registering or initializing the metadata type '{0}' for the target type '{1}.{2}{2}{3}",
                    MetadataType.AssemblyQualifiedName,
                    TargetType.AssemblyQualifiedName,
                    Environment.NewLine,
                    Message);

            return value;
        }
    }

}