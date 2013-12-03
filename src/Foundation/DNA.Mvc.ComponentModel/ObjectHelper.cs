///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Reflection;
using System;

namespace DNA.Web
{
    /// <summary>
    /// The ObjectHelper use to convert the object instance 's properties into a IDictionary object.
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Convert to unknow type object's property names and values into a Dictionary object.
        /// </summary>
        /// <param name="data">The object to be converted.</param>
        /// <returns>A Dictionary object contains the converted object's property names and values.</returns>
        public static IDictionary<string, object> ConvertObjectToDictionary(object data)
        {
            if (data is IDictionary<string,object>)
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
    }
}
