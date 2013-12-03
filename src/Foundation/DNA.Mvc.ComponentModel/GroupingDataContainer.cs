///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DNA.Web
{
    public class GroupingDataContainer
    {
        public object Key { get; set; }
        public bool HasSubgroups { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
        public IEnumerable Items { get; set; }

        public GroupingDataContainer(DynamicGroupResult groupResult) : this(groupResult, 0) { }

        public GroupingDataContainer(DynamicGroupResult groupResult, int level)
        {
            this.Key = groupResult.Key;
            this.Count = groupResult.Count;
            this.Level = level;
            if (groupResult.SubGroups != null)
            {
                this.HasSubgroups = true;
                var items = new List<GroupingDataContainer>();
                foreach (var group in groupResult.SubGroups)
                    items.Add(new GroupingDataContainer(group, this.Level + 1));
                Items = items;
            }
            else
            {
                this.HasSubgroups = false;
                Items = groupResult.Items;
            }
        }
    }
}
