using System.ComponentModel;
using System.Reactive.Concurrency;
using Cafeine.Services;

namespace Cafeine.Services
{
    /// <summary>
    /// DEPRECATED. Use ReactiveProperty instead.
    /// </summary>
    public class CafeineProperty<T> : CafeineScheduler, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CafeineProperty(T val = default(T)) => Value = val;

        private T v;

        public T Value {
            get => v;
            set {
                v = value;
                Scheduler.Schedule(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))));
            }
        }
    }
}
