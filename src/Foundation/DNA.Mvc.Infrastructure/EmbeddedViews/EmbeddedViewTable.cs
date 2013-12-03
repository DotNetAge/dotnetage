//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web.EmbeddedViews
{
    /// <summary>
    /// Represents a cache view table 
    /// </summary>
    [Serializable]
    public class EmbeddedViewTable
    {
        private static readonly object _lock = new object();
        private readonly Dictionary<string, EmbeddedViewMetadata> _viewCache;

        /// <summary>
        /// Initializes a new instance of EmbeddedViewTable class.
        /// </summary>
        public EmbeddedViewTable()
        {
            _viewCache = new Dictionary<string, EmbeddedViewMetadata>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Add view to table.
        /// </summary>
        /// <param name="viewName">The view name.</param>
        /// <param name="assemblyName">The assembly name.</param>
        public void AddView(string viewName, string assemblyName)
        {
            lock (_lock)
            {
                _viewCache[viewName] = new EmbeddedViewMetadata { Name = viewName, AssemblyFullName = assemblyName };
            }
        }

        /// <summary>
        /// Gets all embedded views.
        /// </summary>
        public IList<EmbeddedViewMetadata> Views
        {
            get
            {
                return _viewCache.Values.ToList();
            }
        }

        /// <summary>
        /// Indicates whether the specified view in the table.
        /// </summary>
        /// <param name="fullyQualifiedViewName">The view qualified name.</param>
        /// <returns>true contains otherwise returns false.</returns>
        public bool ContainsEmbeddedView(string fullyQualifiedViewName)
        {
            var foundView = FindEmbeddedView(fullyQualifiedViewName);
            return (foundView != null);
        }

        /// <summary>
        /// Gets embedded view meta data by specified name.
        /// </summary>
        /// <param name="fullyQualifiedViewName">The view qualified name.</param>
        /// <returns>A instance of EmbeddedViewMetadata class.</returns>
        public EmbeddedViewMetadata FindEmbeddedView(string fullyQualifiedViewName)
        {
            //var name = GetNameFromPath(viewPath);
            var name = fullyQualifiedViewName;
            if (string.IsNullOrEmpty(name))
                return null;
            
            //var names = Views.Select(v=>v.Name).ToArray();

            var views = Views.Where(v => v.Name.Contains(fullyQualifiedViewName));
            if (views != null && views.Count() > 0)
            {
                if (views.Count() > 1)
                {
                    //var
                    var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                    var key = "solutioname";
                    if (routeValues.ContainsKey(key))
                    {
                        var solName = (string)routeValues[key];
                        var sol = ModuleRegistration.Modules.Select(k => k.Value).FirstOrDefault(s => s.Name.Equals(solName, StringComparison.OrdinalIgnoreCase));
                        var qualifiedName = sol.AssemblyName+fullyQualifiedViewName;
                        return views.FirstOrDefault(v => v.Name.Equals(qualifiedName));
                    }
                }
                else
                {
                    return views.First();
                }
            }

            return null;

            //return Views.SingleOrDefault(view => view.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
        }
    }
}
