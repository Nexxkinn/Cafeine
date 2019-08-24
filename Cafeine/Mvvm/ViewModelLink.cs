using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;

namespace Cafeine.Services.Mvvm
{
    public class ViewModelLink 
    {
        private static Dictionary<Type, Actions> ActionBase = new Dictionary<Type, Actions>();
        
        public void Publish(Type key) => Publish(payload: null, key: key);

        public void Publish(object payload,Type key)
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
        public void Subscribe<T>(Action<T> action,Type key)
        {
            Actions actions = new Actions();
            actions.Action = new Action<object>(o => action((T)o));
            ActionBase.Add(key, actions);
        }
        public void Subscribe(Action<object> action, Type key)
        {
            Actions actions = new Actions();

            actions.Action = action;

            ActionBase.Add(key, actions);
        }
        public void Subscribe(Action action,Type key)
        {
            Actions actions = new Actions();

            actions.Actiononly = action;

            ActionBase.Add(key, actions);
        }
        public void Unsubscribe(Type key)
        {
            ActionBase.Remove(key);
        }
    }
    internal class Actions
    {
        public Action<object> Action;
        public Action Actiononly;
    }

    // Write all Type here
    public class Keyword { }
    public class SearchBoxVisibility { }
    public class HomePageAvatarLoad { }
}
