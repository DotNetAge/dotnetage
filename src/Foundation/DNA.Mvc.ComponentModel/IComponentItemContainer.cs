///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    /// <summary>
    /// Defines the component could be contains children ViewComponents.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public interface IComponentItemContainer<TComponent>
     where   TComponent:HtmlComponent
    {
        /// <summary>
        /// Gets the children components
        /// </summary>
        IList<TComponent> Items { get; }

        /// <summary>
        /// Trigger when children component added.
        /// </summary>
        /// <param name="item"></param>
        void OnItemAdded(TComponent item);
    }

    ///// <summary>
    ///// Defines the container contains data object.
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    //public interface IDataItemContainer<TModel>
    //    where TModel:class
    //{
    //    TModel DataItem { get; set; }
    //}
}
