//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    public class WidgetCollection : IWidgetCollection
    {
        public WidgetCollection() { this.InnerList = new List<WidgetData>(); }

        protected List<WidgetData> InnerList { get; private set; }

        public void Register<TModule>(string name, string title, string description = "", object preferences = null, string view = "index.cshtml", string category = "utilities") where TModule : class
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(view))
                throw new ArgumentNullException("view");

            if (InnerList.Count(w => w.Name.Equals(name) && w.Category.Equals(category)) > 0)
                throw new Exception(string.Format("There is already has a widget named:\"{0}\" ", name));

            this.InnerList.Add(new WidgetData()
            {
                Category = category,
                Name = name,
                Description = description,
                ModuleType = typeof(TModule),
                Title = title,
                ViewName=view,
                Preferences = preferences != null ? preferences.ToDictionary() : new Dictionary<string, object>()
            });
        }

        public WidgetData this[string name]
        {
            get
            {
                return this.InnerList.FirstOrDefault(w => w.Name.Equals(name));
            }
        }

        public IEnumerator<WidgetData> GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }
    }
}
