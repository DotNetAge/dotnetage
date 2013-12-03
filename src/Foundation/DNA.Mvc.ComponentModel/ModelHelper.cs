///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using DNA.Data;

namespace DNA.Web
{
    public static class ModelHelper
    {
        public static ModelWrapper Query<T>(this IUnitOfWorks works, QueryParams query)
        where T : class
        {
            var datasource = works.All<T>();
            int total = datasource.Count();
            int skipCount = query.GetSkipRecordCount(true);

            IQueryable<T> queryBuilder = null;

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    queryBuilder = datasource.Where(query.GetFilterExpression()).OrderBy(query.GenerateOrderByExpression()).Skip(skipCount).Take(query.Size);
                    total = datasource.Where(query.GetFilterExpression()).Count();
                }
                else
                    queryBuilder = datasource.OrderBy(query.GenerateOrderByExpression()).Skip(skipCount).Take(query.Size);
            }

            string[] groupFields = query.GetGroupByFields();

            if (groupFields != null)
            {
                if (string.IsNullOrEmpty(query.OrderBy))
                {
                    if (!string.IsNullOrEmpty(query.Filter))
                    {
                        queryBuilder = datasource.Where(query.GetFilterExpression()).OrderBy(groupFields[0]).Skip(skipCount).Take(query.Size).AsQueryable();
                        total = datasource.Where(query.GetFilterExpression()).Count();
                    }
                    else
                        queryBuilder = datasource.OrderBy(groupFields[0]).Skip(skipCount).Take(query.Size).AsQueryable();
                }

                return new ModelWrapper
                {
                    Model = queryBuilder.GroupByMany(groupFields).ToList(),
                    Total = total
                };
            }
            else
            {
                if (queryBuilder == null)
                {
                    //var keyField = "";
                    var type = typeof(T);
                    var properties = type.GetProperties();
                    var keyPro = properties.Single(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Count() > 0);

                    if (!string.IsNullOrEmpty(query.Filter))
                    {
                        queryBuilder = datasource.Where(query.GetFilterExpression()).OrderBy(keyPro.Name).Skip(skipCount).Take(query.Size).AsQueryable();
                        total = datasource.Where(query.GetFilterExpression()).Count();
                    }
                    else
                        queryBuilder = datasource.OrderBy(keyPro.Name).Skip(skipCount).Take(query.Size).AsQueryable();
                }
                return new ModelWrapper
                {
                    Model = queryBuilder.ToList(),
                    Total = total
                };
            }


        }

        public static List<GroupingDataContainer> ConvertDataResult(IEnumerable<DynamicGroupResult> result)
        {
            var list = new List<GroupingDataContainer>();
            foreach (var g in result)
                list.Add(new GroupingDataContainer(g));
            return list;
        }

    }
}
