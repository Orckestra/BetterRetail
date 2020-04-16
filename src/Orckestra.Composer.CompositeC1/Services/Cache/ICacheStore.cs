using System;
using System.Collections.Generic;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    public interface ICacheStore<K, V>
    {
        string Name { get; }
        void Add(K key, V value);
        V Get(K key);
        IEnumerable<K> GetKeys();
        void Remove(K key);
        void Clear();

        V GetOrAdd(K key, Func<K, V> loadMethod);
    };
}