using System;
using System.Collections.Concurrent;
using System.Threading;

using Composite;

namespace Hangfire.CompositeC1
{
    public class SimpleLock : IDisposable
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();

        private readonly object _lock;

        private SimpleLock(string resource, TimeSpan timeout)
        {
            Verify.ArgumentNotNullOrEmpty(resource, "resource");

            if (timeout.TotalSeconds > Int32.MaxValue)
            {
                throw new ArgumentException("The timeout specified is greater than Int32.MaxValue when expressed as seconds.", "timeout");
            }

            _lock = Locks.GetOrAdd(resource, new object());

            Monitor.TryEnter(_lock, timeout);
        }

        public static IDisposable AcquireLock(string resource, TimeSpan timeout)
        {
            return new SimpleLock(resource, timeout);
        }

        public void Dispose()
        {
            Monitor.Exit(_lock);
        }
    }
}
