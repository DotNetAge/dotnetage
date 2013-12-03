///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic;

namespace DNA.Web
{
    public class DynamicGroupResult
    {
        public object Key { get; set; }
        public int Count { get; set; }
        public IEnumerable Items { get; set; }
        public IEnumerable<DynamicGroupResult> SubGroups { get; set; }
        public override string ToString() { return string.Format("{0} ({1})", Key, Count); }
    }

    public static class DynamicGroupingExtensions
    {
        public static IEnumerable<DynamicGroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements, params string[] groupSelectors)
        {
            var selectors = new List<Func<TElement, object>>(groupSelectors.Length);
            foreach (var selector in groupSelectors)
            {
                LambdaExpression l =
                    System.Linq.Dynamic.DynamicExpression.ParseLambda(typeof(TElement), typeof(object), selector);
                selectors.Add((Func<TElement, object>)l.Compile());
            }
            return elements.GroupByMany(selectors.ToArray());
        }

        public static IEnumerable<DynamicGroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements, params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray(); //reduce the list recursively until zero
                return
                    elements.GroupBy(selector).Select(
                        g => new DynamicGroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });
            }
            else
                return null;
        }
    }
}
