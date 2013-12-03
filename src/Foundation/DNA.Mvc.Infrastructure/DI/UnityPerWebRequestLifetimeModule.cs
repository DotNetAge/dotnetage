//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Web;

namespace DNA.Web
{

    public class UnityPerWebRequestLifetimeModule : IHttpModule
    {
        private static readonly object Key = new object();
        private HttpContextBase _httpContext;

        //Need this constructor for Unit Test
        public UnityPerWebRequestLifetimeModule(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public UnityPerWebRequestLifetimeModule()
        {
        }

        internal IDictionary<UnityPerWebRequestLifetimeManager, object> Instances
        {
            get
            {
                _httpContext = (HttpContext.Current != null) ? new HttpContextWrapper(HttpContext.Current) : _httpContext;

                return (_httpContext == null) ? null : GetInstances(_httpContext);
            }
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.EndRequest += (sender, e) => RemoveAllInstances();
        }

        void IHttpModule.Dispose()
        {
        }

        internal static IDictionary<UnityPerWebRequestLifetimeManager, object> GetInstances(HttpContextBase httpContext)
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances;

            if (httpContext.Items.Contains(Key))
            {
                instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
            }
            else
            {
                lock (httpContext.Items)
                {
                    if (httpContext.Items.Contains(Key))
                    {
                        instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
                    }
                    else
                    {
                        instances = new Dictionary<UnityPerWebRequestLifetimeManager, object>();
                        httpContext.Items.Add(Key, instances);
                    }
                }
            }

            return instances;
        }

        internal void RemoveAllInstances()
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances = Instances;

            if (instances != null)
            {
                foreach (KeyValuePair<UnityPerWebRequestLifetimeManager, object> entry in instances)
                {
                    IDisposable disposable = entry.Value as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                instances.Clear();
            }
        }
    }
}