namespace Orckestra.Composer.Caching
{
    /// <summary>
    ///     Specifies the relative priority of items stored in the cache.
    /// </summary>
    public enum CacheItemPriority
    {
        /// <summary>
        ///     Normal cache level. This is default value.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     The item can only be removed from the cache according to the item expiration time.
        /// </summary>
        NotRemovable = 1
    }
}
