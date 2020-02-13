using System;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    public class CacheDependentEntry
    {
        public CacheDependentOperations Operations { get; }
        public Type EntityType { get; }

        public CacheDependentEntry(Type entityType)
        {
            Operations = CacheDependentOperations.All;
            EntityType = entityType;
        }

        public CacheDependentEntry(Type entityType, params CacheDependentOperations[] operations)
        {
            foreach (var operation in operations)
            {
                Operations |= operation;
            }

            EntityType = entityType;
        }
    };

    public class CacheDependentEntry<T> : CacheDependentEntry
    {
        public CacheDependentEntry() : base(typeof(T))
        {
        }

        public CacheDependentEntry(params CacheDependentOperations[] operations) : base(typeof(T), operations)
        {
        }
    };
}