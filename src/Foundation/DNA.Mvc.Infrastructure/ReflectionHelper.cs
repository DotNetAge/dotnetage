//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DNA.Web
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Copy the public readable and write property values from source object to destince object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void Copy<T>(T src, T dest, string[] excludes = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            var attr = BindingFlags.Public | BindingFlags.Instance;
            var type = typeof(T);
            var properties = type.GetProperties(attr);
            foreach (var pro in properties)
            {
                if (excludes != null && excludes.Contains(pro.Name))
                    continue;

                if (pro.CanRead && pro.CanWrite)
                    pro.SetValue(dest, pro.GetValue(src, null), null);
            }
        }

        public static void CopyTo<T>(this T src, T dest, params string[] excludes)
            where T : class
        {
            Copy(src, dest, excludes);
        }

        public static T ConvertTo<T>(this object src, params string[] excludes)
            where T:class 
        {
            if (src == null)
                throw new ArgumentNullException("src");

            var dest = Activator.CreateInstance<T>();

            var attr = BindingFlags.Public | BindingFlags.Instance;
            var fromType = src.GetType();
            var toType = typeof(T);

            var fromProps = fromType.GetProperties(attr).Where(t=>t.CanRead && t.CanWrite);
            var toProps = toType.GetProperties(attr).Where(t => t.CanWrite && t.CanWrite);

            foreach (var pro in fromProps)
            {
                if (excludes != null && excludes.Contains(pro.Name))
                    continue;
                
                var toProp = toProps.FirstOrDefault(p => p.Name.Equals(pro.Name));

                if (toProp != null && toProp.CanRead && toProp.CanWrite)
                {
                    var fromVal=pro.GetValue(src, null);
                    toProp.SetValue(dest, fromVal, null);
                }
            }

            return dest;
        }

        public static IDictionary<string, object> ConvertToDictionary(object data)
        {
            if (data is IDictionary<string, object>)
                return data as IDictionary<string, object>;

            var attr = BindingFlags.Public | BindingFlags.Instance;
            var dict = new Dictionary<string, object>();
            foreach (var property in data.GetType().GetProperties(attr))
            {
                if (property.CanRead)
                {
                    dict.Add(property.Name, property.GetValue(data, null));
                }
            }
            return dict;
        }

        public static IDictionary<string, object> ToDictionary(this object data)
        {
            return ConvertToDictionary(data);
        }

    }
}
