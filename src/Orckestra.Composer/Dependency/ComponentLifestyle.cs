namespace Orckestra.Composer.Dependency
{
    /// <summary>
    ///     Define in what scope instances are registered components are reused after initial resolution, and when to dispose
    ///     them.
    ///     This applies to any component or dependency registered through implementations of <see cref="IProviderRegistry" />
    ///     and <see cref="IDependencyRegister" />.
    /// </summary>
    public enum ComponentLifestyle
    {
        /// <summary>
        ///     Transient can be seen as the opposite to singleton. Transient components are not bound to any tangible scope.
        ///     Each time an instance of a transient component is needed, container will produce a new one, never reusing them.
        ///     You can say that the scope of a transient's instance is dictated by its user.
        ///     Therefore transient instances are released when the object using them is released.
        /// </summary>
        Transient = 0,

        /// <summary>
        ///     Instance of a component will be shared in scope of a single web request.
        ///     The instance will be created the first time it's requested in scope of the web request.
        ///     Releasing it explicitly does nothing. Instance will be released upon the end of the web request.
        /// </summary>
        PerRequest = 2,

        /// <summary>
        ///     Singleton components will only produce a single instance that is bound to the container.
        ///     The instance will be created the first time someone requests it, and subsequently reused every time it's needed.
        ///     The sole instance will be released when the container it's registered with is disposed.
        /// </summary>
        Singleton = 1,
    }
}
