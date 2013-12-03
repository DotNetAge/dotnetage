//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the query result returns by using view to execute a query.
    /// </summary>
    /// <example>
    /// <para>The following code example displays the titles of items in a Tasks list where the Status column equals Completed</para>
    /// <code language="cs">
    /// var list=App.Get().CurrentWeb.Lists["tasks"];
    /// var query=new ContentQuery();
    /// query.Eq("Status","Completed")
    ///          .And()
    ///          .Eq(query.SysFieldNames.Owner,HttpContext.Current.User.Identity);
    /// var results= list.DefaultView.Items(query);
    /// foreach (var item in results)
    /// {
    ///   Response.Write(item["Title"].ToString());
    /// }
    /// </code>
    /// </example>
    public class ContentQueryResult : IEnumerable<ContentQueryResultItem>
    {
        /// <summary>
        /// Gets the parent view.
        /// </summary>
        public ContentViewDecorator View { get; internal set; }

        /// <summary>
        /// Gets the parent list object.
        /// </summary>
        public ContentListDecorator List { get; internal set; }

        /// <summary>
        /// Gets query result items.
        /// </summary>
        public IEnumerable<ContentQueryResultItem> Items { get; internal set; }

        /// <summary>
        /// Gets the parent query object 
        /// </summary>
        public ContentQuery Query { get; set; }

        IEnumerator<ContentQueryResultItem> IEnumerable<ContentQueryResultItem>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns the internal css class name
        /// </summary>
        public string CssClass
        {
            get 
            { 
                return this.View.Parent.BaseType + " " + this.View.Parent.Name + " " + this.View.Name; 
            }
        }

        /// <summary>
        /// Sum the field values by specified field name.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns></returns>
        public decimal Sum(string fieldName)
        {
            return this.Items.Where(i=>i[fieldName]!=null && i[fieldName]!=System.DBNull.Value).Sum(i => (decimal)i[fieldName]);
         //   return this.Element().Descendants("row").Where(e => e.Name.Equals(fieldName) && e.Value != null).Select(e => decimal.Parse(e.Value)).Sum();
        }

        /// <summary>
        /// Avg the field values by specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns></returns>
        public decimal Avg(string fieldName)
        {
            return this.Items.Where(i => i[fieldName] != null && i[fieldName] != System.DBNull.Value).Average(i => (decimal)i[fieldName]);
            //return this.Element().Descendants("row").Where(e => e.Name.Equals(fieldName) && e.Value != null).Select(e => decimal.Parse(e.Value)).Average();
        }

        public XElement Element()
        {
            return new XElement(DataNames.Rows, this.Elements());
        }

        /// <summary>
        /// Convert the query result items to XElements.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> Elements()
        {
            var elements = new List<XElement>();
            foreach (var row in Items)
            {
                var element = new XElement(DataNames.Row);
                var data = (IDictionary<string, object>)row.Data;
                foreach (var key in data.Keys)
                {
                    if (ContentViewDecorator.InternalFields.Contains(key))
                        element.Add(new XAttribute(key, row[key]));
                    else
                        element.Add(new XElement(key, row[key]));
                }
                elements.Add(element);
            }
            return elements;
        }
    }
}
