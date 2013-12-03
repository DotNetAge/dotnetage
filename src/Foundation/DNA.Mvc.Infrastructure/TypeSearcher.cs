//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace DNA.Web
{
    /// <summary>
    /// Represets a type helper class use to seach types in all assembilies.
    /// </summary>
    public class TypeSearcher
    {
        //private readonly static TypeSearcher _typeSearcher;

        private TypeSearcher()
        {
            ReloadAssemblies();
        }

        public static TypeSearcher Instance()
        {
            var key = "Sys_Global_TypeSearch";
            var cache = HttpContext.Current.Cache;

            if (cache[key] == null)
            {
                cache.Add(key, new TypeSearcher(), null,
                    DateTime.Now.AddMinutes(2),
                    System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }

            return (TypeSearcher)cache[key];
        }

        /// <summary>
        /// Load all running assemblies and solution assemlblies
        /// </summary>
        public void ReloadAssemblies()
        {
            this.ModuleAssemblies = new List<string>();

            var currentAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.FullName.StartsWith("System.")
                    && !a.FullName.StartsWith("Microsoft.")
                    && !a.FullName.StartsWith("mscorlib,")
                    && !a.FullName.StartsWith("Antlr3.")
                    && !a.FullName.StartsWith("System,")
                    && !a.FullName.StartsWith("CSharpFormat,")
                    && !a.FullName.StartsWith("ICSharpCode.")
                    && !a.FullName.StartsWith("NuGet.")
                    && !a.FullName.StartsWith("EntityFramework,")
                    && !a.FullName.StartsWith("Newtonsoft")
                    && !a.FullName.StartsWith("MySql.")
                    && !a.FullName.StartsWith("WebGrease,")
                    && !a.FullName.StartsWith("WebMatrix.")
                    )
                .ToList();

            var bin = HostingEnvironment.MapPath("~/content/modules/");
            var assemblyFiles = Directory.GetFiles(bin, "*.dll", SearchOption.AllDirectories);

            var asms = assemblyFiles.Select(f => new FileInfo(f))
                .Where(s => !s.Directory.Name.Equals("Bin", StringComparison.OrdinalIgnoreCase) &&
                    !s.Directory.Name.Equals("Debug", StringComparison.OrdinalIgnoreCase) &&
                    !s.Directory.Name.Equals("Temp", StringComparison.OrdinalIgnoreCase) &&
                    !s.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("EntityFramework.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Newtonsoft.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("Antlr3.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("WebGrease.", StringComparison.OrdinalIgnoreCase)
                 && !s.Name.StartsWith("CSharpFormat.", StringComparison.OrdinalIgnoreCase)
                ).ToList();

            if (asms.Count() > 0)
            {
                foreach (var file in asms)
                {
                    var asm = Assembly.LoadFrom(file.FullName);
                    if (asm != null)
                    {
                        if (currentAssemblies.Count(a => a.FullName.Equals(asm.FullName)) == 0)
                        {
                            currentAssemblies.Add(asm);
                            ModuleAssemblies.Add(asm.FullName);
                        }
                    }
                }
            }
            Assemblies = currentAssemblies;
        }

        /// <summary>
        /// Gets all assembiles
        /// </summary>
        public ICollection<Assembly> Assemblies { get; private set; }

        /// <summary>
        /// Gets solution assembile name
        /// </summary>
        public IList<string> ModuleAssemblies { get; private set; }

        /// <summary>
        /// Get specified types in all assemblies.
        /// </summary>
        /// <param name="type">The type to find.</param>
        /// <returns>A collection contains the search type result.</returns>
        public IEnumerable<Type> SearchTypesByType(Type type)
        {
            var typeList = new List<Type>();
            foreach (var asm in Assemblies)
            {
                IEnumerable<Type> types = null;
                try
                {
                    types = asm.GetTypes();
                    typeList.AddRange(types.Where(t => t.Equals(type)));
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return typeList;
        }

        /// <summary>
        /// Get types in all assemblies which has specified custom attrubite types.
        /// </summary>
        /// <param name="attributeType">The custom attribute type.</param>
        /// <param name="inherit"></param>
        /// <returns>A collection contains the search type result.</returns>
        public IEnumerable<Type> SearchTypesOfAttribute(Type attributeType, bool inherit)
        {
            var typeList = new List<Type>();
            foreach (var asm in Assemblies)
            {
                typeList.AddRange(asm.GetTypes().Where(t => t.IsDefined(attributeType, inherit)));
            }
            return typeList;
        }

        /// <summary>
        /// Gets signle custom attribute object from type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T ExtractCustomAttribute<T>(Type type)
        {
            return (T)type.GetCustomAttributes(false).FirstOrDefault(c => c.GetType().Equals(typeof(T)));
        }

        /// <summary>
        /// Gets all descendants types which inhert the specified base type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <returns>A collection contains the search type result.</returns>
        public IEnumerable<Type> SearchTypesByBaseType(Type baseType)
        {
            var typeList = new List<Type>();
            foreach (var asm in Assemblies)
            {
                try
                {
                    if (baseType.IsInterface)
                        typeList.AddRange(asm.GetTypes().Where(t => t.GetInterface(baseType.Name) != null));
                    else
                        typeList.AddRange(asm.GetTypes().Where(t => t.IsSubclassOf(baseType)));
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return typeList;
        }
    }
}
