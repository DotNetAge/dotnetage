///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Web;
using System.Web.Mvc;
//using System.Data.Objects;

namespace DNA.Web
{
    public static class ModelBinder
    {
        //[Obsolete]
        //public static ModelWrapper<TModel> Bind<TModel, TKey>(ObjectQuery<TModel> dataSource, Func<TModel, TKey> keySelector, QueryParams query)
        //{
        //    int total = dataSource.Count();
        //    int skipCount = query.GetSkipRecordCount(true);
        //    List<TModel> data = null;

        //    if (!string.IsNullOrEmpty(query.OrderBy))
        //        data = dataSource.OrderBy(query.GenerateOrderByExpression()).Skip(skipCount).Take(query.Size).ToList();
        //    else
        //        data = dataSource.OrderBy(keySelector).Skip(skipCount).Take(query.Size).ToList();

        //    return new ModelWrapper<TModel>
        //    {
        //        Model = data.ToList(),
        //        Total = total
        //    };
        //}

        //[Obsolete]
        //public static ModelWrapper BindForGrid<TModel, TKey>(ObjectQuery<TModel> dataSource, Func<TModel, TKey> keySelector, QueryParams query)
        //{
        //    int total = dataSource.Count();
        //    int skipCount = query.GetSkipRecordCount(true);
        //    IQueryable<TModel> queryBuilder = null;
        //    IEnumerable result = null;

        //    if (!string.IsNullOrEmpty(query.OrderBy))
        //    {
        //        if (!string.IsNullOrEmpty(query.Filter))
        //        {
        //            queryBuilder = dataSource.Where(query.GetFilterExpression()).OrderBy(query.GenerateOrderByExpression()).Skip(skipCount).Take(query.Size);
        //            total = dataSource.Where(query.GetFilterExpression()).Count();
        //        }
        //        else
        //            queryBuilder = dataSource.OrderBy(query.GenerateOrderByExpression()).Skip(skipCount).Take(query.Size);
        //    }

        //    string[] groupFields = query.GetGroupByFields();

        //    if (groupFields != null)
        //    {
        //        if (string.IsNullOrEmpty(query.OrderBy))
        //        {
        //            if (!string.IsNullOrEmpty(query.Filter))
        //            {
        //                queryBuilder = dataSource.Where(query.GetFilterExpression()).OrderBy("it." + groupFields[0]).Skip(skipCount).Take(query.Size).AsQueryable();
        //                total = dataSource.Where(query.GetFilterExpression()).Count();
        //            }
        //            else
        //                queryBuilder = dataSource.OrderBy("it." + groupFields[0]).Skip(skipCount).Take(query.Size).AsQueryable();
        //        }

        //        result = queryBuilder.GroupByMany(groupFields).ToList();
        //    }
        //    else
        //    {
        //        if (queryBuilder == null)
        //        {
        //            if (!string.IsNullOrEmpty(query.Filter))
        //            {
        //                queryBuilder = dataSource.Where(query.GetFilterExpression()).OrderBy(keySelector).Skip(skipCount).Take(query.Size).AsQueryable();
        //                total = dataSource.Where(query.GetFilterExpression()).Count();
        //            }
        //            else
        //                queryBuilder = dataSource.OrderBy(keySelector).Skip(skipCount).Take(query.Size).AsQueryable();
        //        }

        //        result = queryBuilder.ToList();
        //    }

        //    return new ModelWrapper
        //    {
        //        Model = result,
        //        Total = total
        //    };
        //}

        public static List<GroupingDataContainer> ConvertDataResult(IEnumerable<DynamicGroupResult> result)
        {
            var list = new List<GroupingDataContainer>();
            foreach (var g in result)
                list.Add(new GroupingDataContainer(g));
            return list;
        }
    }
}
