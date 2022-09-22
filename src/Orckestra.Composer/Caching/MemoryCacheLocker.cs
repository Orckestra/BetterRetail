using Orckestra.Composer.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    internal sealed class MemoryCacheLocker
    {
        private static readonly TimeSpan DefaultLockTimeout = TimeSpan.FromSeconds(15);
        private readonly IDictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();
        private readonly IDictionary<string, int> _awaiters = new Dictionary<string, int>();

        public IDisposable Acquire(string key)
        {
            return Acquire(key, DefaultLockTimeout);
        }

        public IDisposable Acquire(string key, TimeSpan timeout)
        {
            SemaphoreSlim ss;
            lock (_semaphores)
            {
                ss = GetOrCreateSemaphore(key);
                _awaiters[key] = _awaiters[key] + 1;
            }

            if (!ss.Wait(timeout))
            {
                HandleWaitTimeout(key, timeout);
            }

            return new MemoryCacheLock(() => OnLockDisposed(key));
        }

        public Task<IDisposable> AcquireAsync(string key)
        {
            return AcquireAsync(key, DefaultLockTimeout);
        }

        public async Task<IDisposable> AcquireAsync(string key, TimeSpan timeout)
        {
            SemaphoreSlim ss;
            lock (_semaphores)
            {
                ss = GetOrCreateSemaphore(key);
                _awaiters[key] = _awaiters[key] + 1;
            }

            if (!(await ss.WaitAsync(timeout).ConfigureAwait(false)))
            {
                HandleWaitTimeout(key, timeout);
            }

            return new MemoryCacheLock(() => OnLockDisposed(key));
        }

        private void HandleWaitTimeout(string key, TimeSpan timeout)
        {
            lock (_semaphores)
            {
                if (_awaiters.ContainsKey(key))
                {
                    _awaiters[key] = _awaiters[key] - 1;
                    if (_awaiters[key] == 0)
                    {
                        RemoveKey(key);
                    }
                }
            }

            throw new TimeoutException("Could not acquire lock after waiting for: " + timeout);
        }

        private void OnLockDisposed(string key)
        {
            lock (_semaphores)
            {
                if (!_awaiters.ContainsKey(key))
                {
                    RemoveKey(key);
                    return;
                }

                _awaiters[key] = _awaiters[key] - 1;

                if (_awaiters[key] == 0)
                {
                    RemoveKey(key);
                }
                else
                {
                    ReleaseSemaphore(key);
                }
            }
        }

        private void RemoveKey(string key)
        {
            AssertLocked();

            DisposeAwaiter(key);
            DisposeSemaphore(key);
        }

        private void DisposeAwaiter(string key)
        {
            AssertLocked();

            if (_awaiters.ContainsKey(key))
            {
                _awaiters.Remove(key);
            }
        }

        private void DisposeSemaphore(string key)
        {
            AssertLocked();

            SemaphoreSlim ss;
            if (_semaphores.TryGetValue(key, out ss))
            {
                ss.Release();
                ss.Dispose();
                _semaphores.Remove(key);
            }
        }

        private void ReleaseSemaphore(string key)
        {
            AssertLocked();

            SemaphoreSlim ss;
            if (_semaphores.TryGetValue(key, out ss))
            {
                ss.Release();
            }
        }

        private SemaphoreSlim GetOrCreateSemaphore(string key)
        {
            AssertLocked();

            if (!_semaphores.ContainsKey(key))
            {
                _semaphores.Add(key, new SemaphoreSlim(1, 1));
                _awaiters.Add(key, 0);
            }

            return _semaphores[key];
        }

        [Conditional("DEBUG")]
        private void AssertLocked()
        {
            Debug.Assert(System.Threading.Monitor.IsEntered(_semaphores));
        }

        private class MemoryCacheLock : IDisposable
        {
            private readonly Action _onDisposed;
            private bool _isDisposed;

            public MemoryCacheLock(Action onDisposed)
            {
                _onDisposed = onDisposed;
            }

            ~MemoryCacheLock() // destructor
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("MemoryCacheLock");
                }

                _isDisposed = true;

                if (_onDisposed != null)
                {
                    _onDisposed();
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
