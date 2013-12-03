//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.Events
{
    /// <summary>
    /// Represent a EventAggregator class to publish and invoke observers.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private Dictionary<string, List<Type>> typeCache = new Dictionary<string, List<Type>>();

        /// <summary>
        /// Publish the specified event.
        /// </summary>
        /// <param name="name">The event name.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event argument object.</param>
        public void Publish(string name, object sender, object eventArgs = null)
        {
            var observers = this.GetObservers(name);
            if (observers != null)
            {
                foreach (var o in observers)
                {
                    try
                    {
                        o.Process(sender, eventArgs);
                    }
                    catch (Exception e) { continue; }
                }
            }
        }

        /// <summary>
        /// Gets the availdable observers.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public IEnumerable<IObserver> GetObservers(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            List<Type> observerTypes = null;

            if (typeCache.ContainsKey(eventName))
            {
                observerTypes = typeCache[eventName];
            }
            else
            {
                var observers = TypeSearcher.Instance().SearchTypesByBaseType(typeof(IObserver));
                if (observers != null)
                    observerTypes = observers.Where(o => !o.IsAbstract && !o.IsInterface && o.IsDefined(typeof(BindToAttribute), false) && ((BindToAttribute)o.GetCustomAttributes(typeof(BindToAttribute), false).First()).EventName.Equals(eventName)).ToList();
                this.typeCache.Add(eventName, observerTypes);
            }


            if (observerTypes != null)
            {
                var instances = new List<IObserver>();

                foreach (var _type in observerTypes)
                {
                    try
                    {
                        var instance = Activator.CreateInstance(_type) as IObserver;
                        if (instance != null)
                            instances.Add(instance);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }

                return instances.OrderByDescending(i => i.Order).ToList();

            }

            return null;
        }
    }
}
