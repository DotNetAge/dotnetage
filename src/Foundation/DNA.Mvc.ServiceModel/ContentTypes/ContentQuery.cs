//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the query in list view.
    /// </summary>
    /// <example>
    /// <para>The following code example displays the titles of items in a Tasks list where the Status column equals Completed</para>
    /// <code language="cs">
    /// var list=App.Get().CurrentWeb.Lists["tasks"];
    /// var query=new ContentQuery();
    /// query.Eq("Status","Completed")
    ///          .And()
    ///          .Eq(query.SysFieldNames.Owner,HttpContext.Current.User.Identity);
    /// var items= list.DefaultView.Items(query);
    /// foreach (var item in items)
    /// {
    ///   Response.Write(item["Title"].ToString());
    /// }
    /// </code>
    /// </example>
    public class ContentQuery
    {
        private int index = 1;

        /// <summary>
        /// Initializes a new instance of the ContentQuery class.
        /// </summary>
        public ContentQuery() { }

        /// <summary>
        /// Initializes a new instance of the ContentQuery class with http request.
        /// </summary>
        /// <remarks>
        /// This constructor will parse the query string and route data to init the query object.
        /// </remarks>
        /// <param name="request">The http request object.</param>
        public ContentQuery(HttpRequestBase request)
        {
            var queryStr = request.QueryString;

            if (!string.IsNullOrEmpty(queryStr["index"]))
                Index = int.Parse(queryStr["index"]);

            if (!string.IsNullOrEmpty(queryStr["size"]))
                Size = int.Parse(queryStr["size"]);

            if (!string.IsNullOrEmpty(queryStr["filter"]))
                Filter = queryStr["filter"];

            if (!string.IsNullOrEmpty(queryStr["sort"]))
                Sort = queryStr["sort"];

            if (!string.IsNullOrEmpty(queryStr["groupby"]))
                GroupBy = queryStr["groupby"];

            if (!string.IsNullOrEmpty(Sort))
                Sort = Sort.Replace("-", ",").Replace("~", " ");

            if (!string.IsNullOrEmpty(Filter))
                Filter = GetFilterExpression();
        }

        /// <summary>
        /// Gets the query result items count.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets / Sets the query paging index.
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                if (value <= 1)
                    index = 1;
                else
                    index = value;
            }
        }

        /// <summary>
        /// Gets / Sets the query result paging size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets / Sets the filter expression.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets/Sets sort by expression.
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Gets/Sets group by expression.
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// Gets the total page count of current query.
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (Size == 0)
                    return 0;
                return (int)Math.Ceiling((decimal)Total / (decimal)Size);
            }
        }

        /// <summary>
        /// Parse the sort expression
        /// </summary>
        /// <returns>A sort param collection that contains the sort expression parsing result.</returns>
        public List<SortParam> GetSortParams()
        {
            if (!string.IsNullOrEmpty(Sort))
            {
                return Sort.Split(',').Select(s => new SortParam(s)).ToList();
            }
            return null;
        }

        /// <summary>
        /// Get filter expression from query string.
        /// </summary>
        /// <returns></returns>
        public string GetFilterExpression()
        {
            if (!string.IsNullOrEmpty(Filter))
            {
                var filterExprs = HttpContext.Current.Server.UrlDecode(Filter).Split('-');
                var exprs = new List<string>();

                foreach (var expr in filterExprs)
                {
                    var args = expr.Split('~');
                    var fieldName = args[0];

                    var oper = args.Length == 1 ? expr : args[1].ToLower();

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        if (oper == "startswith")
                        {
                            exprs.Add(fieldName + " LIKE '" + args[2] + "%'");
                            continue;
                        }

                        if (oper == "endswith")
                        {
                            exprs.Add(fieldName + " LIKE '%" + args[2] + "'");
                            continue;
                        }

                        if (oper == "contains")
                        {
                            exprs.Add(fieldName + " LIKE '%" + args[2] + "%'");
                            continue;
                        }

                        if (oper == "eq")
                        {
                            exprs.Add(fieldName + "=" + args[2]);
                            continue;
                        }

                        if (oper == "neq")
                        {
                            exprs.Add(fieldName + "<>" + args[2]);
                            continue;
                        }

                        if (oper == "lt")
                        {
                            exprs.Add(fieldName + "<" + args[2]);
                            continue;
                        }

                        if (oper == "le")
                        {
                            exprs.Add(fieldName + "<=" + args[2]);
                            continue;
                        }

                        if (oper == "gt")
                        {
                            exprs.Add(fieldName + ">" + args[2]);
                            continue;
                        }

                        if (oper == "ge")
                        {
                            exprs.Add(fieldName + ">=" + args[2]);
                            continue;
                        }

                        if (oper == "and" || oper == "or" || oper == "not")
                            exprs.Add(oper);
                    }

                    //exprs.Add("(" + expr.Replace(fieldName, "[Extent1].[" + fieldName+"]") + ")");
                }

                string result = string.Join(" ", exprs.ToArray());
                return result;
            }
            return "";
        }

        /// <summary>
        /// Get filter expressions.
        /// </summary>
        /// <returns></returns>
        public List<FilterExpression> GetFilterExpressions()
        {
            var exprs = new List<FilterExpression>();

            if (!string.IsNullOrEmpty(Filter))
            {
                var filterExprs = Filter.Split('-');
                for (int i = 0; i < filterExprs.Length; i++)
                {
                    exprs.Add(new FilterExpression(filterExprs[i]));
                }
            }

            return exprs;
        }

        #region Helper methods

        /// <summary>
        /// Add "AND" boolean exprssion to current filter.
        /// </summary>
        /// <returns>Current query object.</returns>
        public ContentQuery And()
        {
            if (!string.IsNullOrEmpty(Filter))
                Filter = string.Format("(0)", Filter) + " AND ";
            return this;
        }

        /// <summary>
        /// Add "OR" boolean exprssion to current filter.
        /// </summary>
        /// <returns>Current query object.</returns>
        public ContentQuery Or()
        {
            if (!string.IsNullOrEmpty(Filter))
                Filter = string.Format("(0)", Filter) + " OR ";
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified boolean value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The boolean value</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Not(string fieldName, bool val = true)
        {
            Filter += fieldName + "<>" + (val ? "True" : "False");
            return this;
        }

        #region Equal

        /// <summary>
        /// Add filter expression that the field value equal to the specified string value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The string value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, string val)
        {
            Filter += string.Format("{0}='{1}'", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value equal to the specified datetime value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The datetime value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, DateTime val)
        {
            Filter += string.Format("{0}=#{1}#", fieldName, val.ToString("yyyy-MM-dd"));
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value equal to the specified integer value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The integer value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, int val)
        {
            Filter += string.Format("{0}={1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value equal to the specified float value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The float value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, float val)
        {
            Filter += string.Format("{0}={1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value equal to the specified decimal value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The decimal value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, decimal val)
        {
            Filter += string.Format("{0}={1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value equal to the specified double value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The double value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Eq(string fieldName, double val)
        {
            Filter += string.Format("{0}={1}", fieldName, val);
            return this;
        }

        #endregion

        #region Not equal

        /// <summary>
        ///  Add filter expression that the field value not equal to the specified string value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The string value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, string val)
        {
            Filter += string.Format("{0}<>'{1}'", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified DateTime value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The DateTime value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, DateTime val)
        {
            Filter += string.Format("{0}<>#{1}#", fieldName, val.ToString("yyyy-MM-dd"));
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified integer value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The integer value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, int val)
        {
            Filter += string.Format("{0}<>{1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified float value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The float value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, float val)
        {
            Filter += string.Format("{0}<>{1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified decimal value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The decimal value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, decimal val)
        {
            Filter += string.Format("{0}<>{1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value not equal to the specified double value.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The double value.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Neq(string fieldName, double val)
        {
            Filter += string.Format("{0}<>{1}", fieldName, val);
            return this;
        }

        #endregion

        /// <summary>
        /// Add filter expression that the field value less than the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val"></param>
        /// <returns>Current query object.</returns>
        public ContentQuery Lt<T>(string fieldName, T val)
        {
            if (typeof(T) == typeof(DateTime))
                Filter += string.Format("{0}<#{1}#", fieldName, val);
            else
                Filter += string.Format("{0}<{1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value less than and equal the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val"></param>
        /// <returns>Current query object.</returns>
        public ContentQuery Lte<T>(string fieldName, T val)
        {
            if (typeof(T) == typeof(DateTime))
                Filter += string.Format("{0}<=#{1}#", fieldName, val);
            else
                Filter += string.Format("{0}<={1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value great than the specified decimal value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val"></param>
        /// <returns>Current query object.</returns>
        public ContentQuery Gt<T>(string fieldName, T val)
        {
            if (typeof(T) == typeof(DateTime))
                Filter += string.Format("{0}>#{1}#", fieldName, val);
            else
                Filter += string.Format("{0}>{1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add filter expression that the field value great than and equal the specified decimal value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val"></param>
        /// <returns>Current query object.</returns>
        public ContentQuery Gte<T>(string fieldName, T val)
        {
            if (typeof(T) == typeof(DateTime))
                Filter += string.Format("{0}>=#{1}#", fieldName, val);
            else
                Filter += string.Format("{0}<={1}", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add a expression to filter that the file value starts with the specified search term.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The string search term.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery StartsWith(string fieldName, string val)
        {
            Filter += string.Format("{0} LIKE '{1}%'", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add a expression to filter that the file value ends with the specified search term.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The string search term.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery EndsWith(string fieldName, string val)
        {
            Filter += string.Format("{0} LIKE '%{1}'", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add a expression to filter that specified field value contains the search term.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="val">The string search term.</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Contains(string fieldName, string val)
        {
            Filter += string.Format("{0} LIKE '%{1}%'", fieldName, val);
            return this;
        }

        /// <summary>
        /// Add sort expression by specified field name and direction.
        /// </summary>
        /// <param name="fieldName">The field name that exists in view fields.</param>
        /// <param name="dir">The sort direction</param>
        /// <returns>Current query object.</returns>
        public ContentQuery Sortby(string fieldName, ContentQuerySorts dir = ContentQuerySorts.Asc)
        {
            var exp = fieldName + (dir == ContentQuerySorts.Asc ? "" : "desc");
            Sort += !string.IsNullOrEmpty(Sort) ? "," : "" + exp;
            return this;
        }

        #endregion

        private SystemFields sys_fields = new SystemFields();
        
        /// <summary>
        /// Gets the system defined names that use in filter,sort and group by expressions.
        /// </summary>
        public SystemFields SysFieldNames
        {
            get { return sys_fields; }
        }

    }

    public class SystemFields
    {
        public string ParentID { get { return DataNames.ParentID; } }
        public string Privacy { get { return DataNames.Privacy; } }
        public string Created { get { return DataNames.Created; } }
        public string Modified { get { return DataNames.Modified; } }
        public string Published { get { return DataNames.Published; } }
        public string Position { get { return DataNames.Pos; } }
        public string State { get { return DataNames.State; } }
        public string IsPublished { get { return DataNames.IsPublished; } }
        public string EnableComments { get { return DataNames.EnableComments; } }
        public string Slug { get { return DataNames.Slug; } }
        public string Path { get { return DataNames.Path; } }
        public string Tags { get { return DataNames.Tags; } }
        public string Owner { get { return DataNames.Owner; } }
        public string Modifier { get { return DataNames.Modifier; } }
        public string Ratings { get { return DataNames.Ratings; } }
        public string Reads { get { return DataNames.Reads; } }
        public string TotalAttachs { get { return DataNames.TotalAttachs; } }
        public string TotalVotes { get { return DataNames.TotalVotes; } }
        public string TotalComms { get { return DataNames.TotalComms; } }
        public string TotalShares { get { return DataNames.TotalShares; } }
        public string Version { get { return DataNames.Version; } }
        public string HasChildren { get { return DataNames.HasChildren; } }
    }

    public enum ContentQuerySorts
    {
        Asc,
        Desc
    }

    /// <summary>
    /// Represents the sort parameter.
    /// </summary>
    public class SortParam
    {
        /// <summary>
        /// Initialize the SortParam object by sepcified sort expression.
        /// </summary>
        /// <param name="expr"></param>
        public SortParam(string expr)
        {
            var args = expr.Split('~');
            if (args.Length == 1)
                args = expr.Split(' ');

            this.FieldName = args[0];
            this.Direction = args[1].ToUpper();
        }

        /// <summary>
        /// Gets/Sets the field name that to be sorted.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets / Sets the sort direction.
        /// </summary>
        public string Direction { get; set; }

    }
}
