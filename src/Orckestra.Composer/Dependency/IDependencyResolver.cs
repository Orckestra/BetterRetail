using System;

namespace Orckestra.Composer.Dependency
{
    /// <summary>
    ///     This interface defines the contract that must be implemented by any dependency resolver from a registry.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        ///     Resolves a registered instance of type T
        /// </summary>
        /// <typeparam name="T">
        ///     Any type.
        /// </typeparam>
        /// <returns>
        ///     An instance of type T.
        /// </returns>
        /// <remarks>Attempting to resolve an unregistered component will throw a registry implementation specific exception.</remarks>
        T Resolve<T>();

        /// <summary>
        ///     Tries to resolve a registered instance of type T.
        /// </summary>
        /// <typeparam name="T">
        ///     Any class implementation.
        /// </typeparam>
        /// <returns>
        ///     An instance of type T if registered type was found otherwise null.
        /// </returns>
        T TryResolve<T>() where T : class;

        /// <summary>
        /// Resolves a registered instance of the  provided type.
        /// </summary>
        /// <param name="type">
        /// Any type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> instance.
        /// </returns>
        /// <remarks>
        /// Attempting to resolve an unregistered component will throw a registry implementation specific exception.
        /// </remarks>
        object Resolve(Type type);

        /// <summary>
        /// Tries to resolve a registered instance of the provided type.
        /// </summary>
        /// <param name="type">
        /// Any type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> instance if provided type was registered otherwise null.
        /// </returns>
        object TryResolve(Type type);
    }
}
