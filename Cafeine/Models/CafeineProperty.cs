using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Models
{
    public class CafeineProperty<T> : INotifyPropertyChanged
    {
        public CafeineProperty(T val = default(T)) => Value = val;

        // Adapted from Reactive Property.
        private static Lazy<SynchronizationContextScheduler> synccontext { get; } =
            new Lazy<SynchronizationContextScheduler>(() =>
            {
                if (SynchronizationContext.Current == null)
                {
                    throw new InvalidOperationException("SynchronizationContext.Current is null");
                }

                return new SynchronizationContextScheduler(SynchronizationContext.Current);
            });
        
        public static IScheduler Scheduler => synccontext.Value;

        public event PropertyChangedEventHandler PropertyChanged;

        private T v;

        public T Value {
            get => v;
            set {
                v = value;
                Scheduler.Schedule(()=> PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(Value))));
            }
        }
    }
}
