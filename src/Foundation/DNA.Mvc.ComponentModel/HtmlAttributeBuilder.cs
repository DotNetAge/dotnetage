///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DNA.Web
{

    /// <summary>
    /// The base class use to build html attributes.
    /// </summary>
    public abstract class HtmlAttributeBuilder
    {
        private Dictionary<string, object> htmlAttributes;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        protected Dictionary<string, object> InnerAttributes
        {
            get
            {
                if (htmlAttributes == null)
                    htmlAttributes = new Dictionary<string, object>();
                return htmlAttributes;
            }
        }

        public void MergeAttribute(string key, string value)
        {
            this.MergeAttribute(key, value, false);
        }

        public void MergeAttribute(string key, string value, bool replaceExisting)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (replaceExisting || !this.InnerAttributes.ContainsKey(key))
                this.InnerAttributes[key] = value;
        }

        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        {
            this.MergeAttributes<TKey, TValue>(attributes, false);
        }

        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
        {
            if (attributes != null)
            {
                foreach (KeyValuePair<TKey, TValue> pair in attributes)
                {
                    string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
                    string str2 = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                    this.MergeAttribute(key, str2, replaceExisting);
                }
            }
        }

    }
}
