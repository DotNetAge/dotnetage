///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Linq.Dynamic;
using System.Collections;
using DNA.Data;

namespace DNA.Web
{
    [Serializable]
    public class QueryParams
    {
        private int pageSize = 20;
        private int pageIndex = 1;

        public string OrderBy { get; set; }

        public string GroupBy { get; set; }

        public string Filter { get; set; }

        public int Index
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        public int Size
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        public string GetSortExpression()
        {
            var fields = GetSortingFields();
            var ordered = new List<string>();

            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (GetSortingOrder(fields[i]) == SortingOrders.Asc)
                        ordered.Add(fields[i].Split('~')[0]);
                    else
                        ordered.Add(fields[i].Split('~')[0] + " desc");
                }
                if (ordered.Count > 0)
                    return string.Join(",", ordered.ToArray());
            }
            return "";
        }

        public string GenerateOrderByExpression()
        {
            var fields = GetSortingFields();
            var ordered = new List<string>();

            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (GetSortingOrder(fields[i]) == SortingOrders.Asc)
                        ordered.Add("it." + fields[i].Split('~')[0]);
                    else
                        ordered.Add("it." + fields[i].Split('~')[0] + " desc");
                }
                if (ordered.Count > 0)
                    return string.Join(",", ordered.ToArray());
            }
            return "";
        }

        /// <summary>
        /// Convert the querstring filter field to filter expression format.
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
                    var oper = args[1];

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        if (oper == "startswith")
                        {
                            exprs.Add(fieldName + ".StartsWith(" + args[2] + ")");
                            continue;
                        }

                        if (oper == "endswith")
                        {
                            exprs.Add(fieldName + ".EndsWith(" + args[2] + ")");
                            continue;
                        }

                        if (oper == "contains")
                        {
                            exprs.Add(fieldName + ".Contains(" + args[2] + ")");
                            continue;
                        }

                        if (oper == "eq")
                        {
                            exprs.Add(fieldName + "==" + args[2]);
                            continue;
                        }

                        if (oper == "neq")
                        {
                            exprs.Add(fieldName + "!=" + args[2]);
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
                    }

                    //exprs.Add("(" + expr.Replace(fieldName, "[Extent1].[" + fieldName+"]") + ")");
                }

                string result = string.Join(" && ", exprs.ToArray());
                //result = result.Replace("~lt~", "<")
                //    .Replace("~gt~", ">")
                //    .Replace("~le~", "<=")
                //    .Replace("~ge~", ">=")
                //    .Replace("~eq~", "=")
                //    .Replace("~neq~", "!=")
                //    //.Replace("~startswith~", " like ")
                //    //.Replace("~endswith~", " like ")
                //    //.Replace("~contains~", " like ")
                //    .Replace("~and~", " && ")
                //    .Replace("~or~", " || ")
                //    .Replace("~not~", " !");
                return result;
            }
            return "";
        }

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

        public string[] GetSortingFields()
        {
            if (!string.IsNullOrEmpty(OrderBy))
            {
                return OrderBy.Split(new char[] { '-' });
            }
            else
                return null;
        }

        public SortingOrders GetSortingOrder(string expr)
        {
            var sortingParams = expr.Split(new char[] { '~' });
            if (sortingParams.Count() == 2)
                return string.IsNullOrEmpty(sortingParams[1]) ? SortingOrders.Asc : (sortingParams[1].Equals("asc", StringComparison.OrdinalIgnoreCase) ? SortingOrders.Asc : SortingOrders.Desc);
            else
                return SortingOrders.Asc;
        }

        public int GetZeroBaseIndex()
        {
            return Index == 0 ? Index : Index - 1;
        }

        public string[] GetGroupByFields()
        {
            if (!string.IsNullOrEmpty(GroupBy))
            {
                var _fieldArgs = GroupBy.Split(new char[] { '-' });
                return _fieldArgs;
            }
            return null;
        }

        public int GetSkipRecordCount(bool? isZeroBase = false)
        {
            if (isZeroBase.Value)
                return GetZeroBaseIndex() * Size;
            else
                return Index * Size;
        }

        public bool HasOrders { get { return !string.IsNullOrEmpty(OrderBy); } }

        public bool HasFilters { get { return !string.IsNullOrEmpty(Filter); } }

        public bool HasGroups { get { return !string.IsNullOrEmpty(GroupBy); } }

        /// <summary>
        /// Apply the sorting options and get an IQueryable result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IQueryable<T> GetOrderingResult<T>(IQueryable<T> source)
        {
            if (HasOrders)
                return source.OrderBy(GenerateOrderByExpression());
            else
                return source;
        }

        /// <summary>
        /// Apply the paging options and get an IQueryable object result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="isZerobase"></param>
        /// <returns></returns>
        public IQueryable<T> GetPageResult<T>(IQueryable<T> source, bool isZerobase = false)
            where T : class
        {
            var skipCount = GetSkipRecordCount(isZerobase);
            if (skipCount == 0)
                return source.Take(Size).AsQueryable();
            else
                return source.Skip(skipCount).Take(Size).AsQueryable();
        }

        public IQueryable<T> GetFilterResult<T>(IQueryable<T> source)
            where T : class
        {
            if (HasFilters)
                return source.Where(GetFilterExpression());
            else
                return source;
        }

        public IEnumerable GetGroupResult<T>(IQueryable<T> source)
            where T : class
        {
            if (HasGroups)
                return source.GroupByMany(GetGroupByFields());
            else
                return source;
        }
    }
}
