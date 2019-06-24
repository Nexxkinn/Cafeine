using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Cafeine.Services
{
    public class CafeineScheduler
    {
        public static IScheduler Scheduler => synccontext.Value;

        // Adapted from neuecc's ReactiveProperty.
        private static Lazy<SynchronizationContextScheduler> synccontext { get; } =
            new Lazy<SynchronizationContextScheduler>(() =>
            {
                if (SynchronizationContext.Current == null)
                {
                    throw new InvalidOperationException("SynchronizationContext.Current is null");
                }

                return new SynchronizationContextScheduler(SynchronizationContext.Current);
            });
    }
}
