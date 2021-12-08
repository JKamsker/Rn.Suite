using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rnd.Lib.Extensions
{
    public static class SemaphoreExtensions
    {
        public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellation = default)
        {
            await semaphoreSlim.WaitAsync(cancellation);
            return new SemaphoreReleaser(semaphoreSlim);
        }

        private class SemaphoreReleaser : IDisposable
        {
            private SemaphoreSlim _semaphoreSlim;

            public SemaphoreReleaser(SemaphoreSlim semaphoreSlim)
            {
                _semaphoreSlim = semaphoreSlim;
            }

            public void Dispose()
            {
                _semaphoreSlim?.Release(1);
                _semaphoreSlim = null;
            }
        }
    }
}