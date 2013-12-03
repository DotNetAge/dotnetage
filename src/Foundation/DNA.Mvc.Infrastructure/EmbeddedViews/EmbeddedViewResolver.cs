//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Reflection;

namespace DNA.Web.EmbeddedViews
{
    public class EmbeddedViewResolver
    {
        public EmbeddedViewTable GetEmbeddedViews()
        {
            var assemblies = TypeSearcher.Instance().Assemblies;

            if (assemblies == null || assemblies.Count == 0) return null;

            var table = new EmbeddedViewTable();

            foreach (var assembly in assemblies)
            {
                var names = GetNamesOfAssemblyResources(assembly);
                if (names == null || names.Length == 0) continue;

                foreach (var name in names)
                {
                    var key = name.ToLowerInvariant();
                    
                    if (!key.Contains(".views.") && !key.Contains(".widgets.")) 
                        continue;

                    table.AddView(name, assembly.FullName);
                }
            }

            return table;
        }

        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            //GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
            try
            {
                if (!assembly.IsDynamic)
                    return assembly.GetManifestResourceNames();
            }
            catch
            {
                // Any exception we fall back to returning an empty array.
            }

            return new string[] { };
        }
    }
}
