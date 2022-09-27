using System;

namespace Orckestra.Composer.Dependency
{
    /// <summary>
    ///     This interface defines the contract that must be implemented by any registry to allow dependencies registration.
    /// </summary>
    public interface IDependencyRegister
    {
        /// <summary>
        /// Registers a single instance of an object as an implementation of type TA.
        /// </summary>
        /// <typeparam name="TAs">
        /// Any type.
        /// </typeparam>
        /// <param name="instance">
        /// The instance to register.
        /// </param>
        void Register<TAs>(object instance);

        /// <summary>
        ///     Registers a type T as an implementation of type TAs.
        /// </summary>
        /// <typeparam name="T">
        ///     Any type implementation of TAs.
        /// </typeparam>
        /// <typeparam name="TAs">
        ///     Any type.
        /// </typeparam>
        /// <remarks>Type will be registered with <see cref="ComponentLifestyle.Transient" /></remarks>
        void Register<T, TAs>() where T : TAs;

        /// <summary>
        /// Registers a type T as an implementation of type TAs with the provided lifestyle.
        /// </summary>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        /// <typeparam name="T">
        /// Any type implementation of TAs.
        /// </typeparam>
        /// <typeparam name="TAs">
        /// Any type.
        /// </typeparam>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lifestyle value was not recognized
        /// </exception>
        void Register<T, TAs>(ComponentLifestyle lifestyle) where T : TAs;

        /// <summary>
        /// Registers a type implementationType as an implementation of type asType with the provided lifestyle.
        /// </summary>
        /// <param name="implementationType">
        /// Any type implementation of asType.
        /// </param>
        /// <param name="asType">
        /// Any type.
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lifestyle value was not recognized
        /// </exception>
        void Register(Type implementationType, Type asType, ComponentLifestyle lifestyle);

        /// <summary>
        /// Registers a type implementationType as an implementation of all type asType with the provided lifestyle. 
        /// </summary>
        /// <param name="implementationType">
        /// Type of the implementation.
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        /// <param name="asTypes">
        /// Any type.
        /// </param>
        void Register(Type implementationType, ComponentLifestyle lifestyle, params Type[] asTypes);

        /// <summary>
        /// Registers a type implementationType as an implementation of type asType with the provided lifestyle.
        /// </summary>
        /// <param name="implementationType">
        /// Any type implementation of asType.
        /// </param>
        /// <param name="asType">
        /// Any type.
        /// </param>
        /// <remarks>
        /// Type will be registered with <see cref="ComponentLifestyle.Transient"/>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lifestyle value was not recognized
        /// </exception>
        void Register(Type implementationType, Type asType);

        /// <summary>
        /// Registers a type implementationType as an implementation of itself with the provided lifestyle.
        /// </summary>
        /// <param name="implementationType">
        /// Any type.
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lifestyle value was not recognized
        /// </exception>
        void Register(Type implementationType, ComponentLifestyle lifestyle);

        /// <summary>
        /// Registers a type implementationType as an implementation of itself.
        /// </summary>
        /// <param name="implementationType">
        /// Any type.
        /// </param>
        /// <remarks>
        /// Type will be registered with <see cref="ComponentLifestyle.Transient"/>
        /// </remarks>
        void Register(Type implementationType);

        /// <summary>
        ///     Registers type T as implementation of itself.
        /// </summary>
        /// <typeparam name="T">
        ///     Any type.
        /// </typeparam>
        /// <remarks>Type will be registered with <see cref="ComponentLifestyle.Transient" /></remarks>
        void Register<T>();

        /// <summary>
        /// Registers type T as implementation of itself.
        /// </summary>
        /// <typeparam name="T">
        /// Any type.
        /// </typeparam>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lifestyle value was not recognized
        /// </exception>
        void Register<T>(ComponentLifestyle lifestyle);
    }

    /// <summary>
    /// This class extends some of the base functionality of the <see cref="IDependencyRegister"/>.
    /// </summary>
    public static class ExtendIDependencyRegister
    {
        /// <summary>
        /// Registers a type implementationType as an implementation of all type asType with the provided lifestyle. 
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the implementation that will be returned when resolution is done for type types TAs0...TAsN.
        /// </typeparam>
        /// <typeparam name="TAs0">
        /// A first <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs1">
        /// A second <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <param name="this">
        /// An instance implementing <see cref="IDependencyRegister"/>
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        public static void Register<T, TAs0, TAs1>(this IDependencyRegister @this, ComponentLifestyle lifestyle)
        {
            @this.Register(typeof(T), lifestyle, typeof(TAs0), typeof(TAs1));
        }

        /// <summary>
        /// Registers a type implementationType as an implementation of all type asType with the provided lifestyle. 
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the implementation that will be returned when resolution is done for type types TAs0...TAsN.
        /// </typeparam>
        /// <typeparam name="TAs0">
        /// A first <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs1">
        /// A second <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs2">
        /// A third <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <param name="this">
        /// An instance implementing <see cref="IDependencyRegister"/>
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        public static void Register<T, TAs0, TAs1, TAs2>(this IDependencyRegister @this, ComponentLifestyle lifestyle)
        {
            @this.Register(typeof(T), lifestyle, typeof(TAs0), typeof(TAs1), typeof(TAs2));
        }

        /// <summary>
        /// Registers a type implementationType as an implementation of all type asType with the provided lifestyle. 
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the implementation that will be returned when resolution is done for type types TAs0...TAsN.
        /// </typeparam>
        /// <typeparam name="TAs0">
        /// A first <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs1">
        /// A second <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs2">
        /// A third <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs3">
        /// A fourth <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <param name="this">
        /// An instance implementing <see cref="IDependencyRegister"/>
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        public static void Register<T, TAs0, TAs1, TAs2, TAs3>(this IDependencyRegister @this, ComponentLifestyle lifestyle)
        {
            @this.Register(typeof(T), lifestyle, typeof(TAs0), typeof(TAs1), typeof(TAs2), typeof(TAs3));
        }

        /// <summary>
        /// Registers a type implementationType as an implementation of all type asType with the provided lifestyle. 
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the implementation that will be returned when resolution is done for type types TAs0...TAsN.
        /// </typeparam>
        /// <typeparam name="TAs0">
        /// A first <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs1">
        /// A second <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs2">
        /// A third <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs3">
        /// A fourth <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <typeparam name="TAs4">
        /// A fifth <see cref="Type"/> for which an instance of type T will be resolved.
        /// </typeparam>
        /// <param name="this">
        /// An instance implementing <see cref="IDependencyRegister"/>
        /// </param>
        /// <param name="lifestyle">
        /// The lifestyle for resolution.
        /// </param>
        public static void Register<T, TAs0, TAs1, TAs2, TAs3, TAs4>(this IDependencyRegister @this, ComponentLifestyle lifestyle)
        {
            @this.Register(typeof(T), lifestyle, typeof(TAs0), typeof(TAs1), typeof(TAs2), typeof(TAs3), typeof(TAs4));
        }
    }
}
