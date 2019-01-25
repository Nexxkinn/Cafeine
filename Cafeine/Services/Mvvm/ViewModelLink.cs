using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cafeine.Services.Mvvm
{
    public class ViewModelLink
    {
        private static Dictionary<string, Actions> ActionBase = new Dictionary<string, Actions>();

        private static readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        public void Publish(string key) => Publish(payload: null, key: key);

        public void Publish(object payload,string key)
        {
            if (ActionBase.ContainsKey(key))
            {
                var action = ActionBase[key];
                if (action.Action == null)
                {
                    action.Actiononly();
                }
                else
                {
                    action.Action(payload);
                }
            }
        }
        public void Subscribe<T>(Action<T> action,string key)
        {
            Actions actions = new Actions();
            actions.Action = new Action<object>(o => action((T)o));
            ActionBase.Add(key, actions);
        }
        public void Subscribe(Action<object> action, string key)
        {
            Actions actions = new Actions();

            actions.Action = action;

            ActionBase.Add(key, actions);
        }
        public void Subscribe(Action action,string key)
        {
            Actions actions = new Actions();

            actions.Actiononly = action;

            ActionBase.Add(key, actions);
        }
        public void Unsubscribe(string key)
        {
            ActionBase.Remove(key);
        }
    }
    internal class Actions
    {
        public Action<object> Action;
        public Action Actiononly;
    }
}
