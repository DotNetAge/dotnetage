///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    public abstract class ContainerViewComponent : ContainerViewComponent<TemplateViewComponent> { }

    /// <summary>
    /// Defines the base class to contain component items.
    /// </summary>
    public abstract class ContainerViewComponent<TComponent> : HtmlComponent, IComponentItemContainer<TComponent>
        where TComponent : HtmlComponent
    {
        private IList<TComponent> views;

        /// <summary>
        /// Gets/Sets the children component items.
        /// </summary>
        protected IList<TComponent> InnerItems
        {
            get
            {
                if (views == null)
                    views = new List<TComponent>();
                return views;
            }
            set
            {
                views = value;
            }
        }

        /// <summary>
        /// Add children item
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(TComponent item)
        {
            InnerItems.Add(item);
            this.OnItemAdded(item);
        }
        
        IList<TComponent> IComponentItemContainer<TComponent>.Items
        {
            get { return this.InnerItems; }
        }

        protected virtual void OnItemAdded(TComponent item) { }

        public override void RenderContent(System.Web.UI.HtmlTextWriter writer)
        {
            foreach (var item in InnerItems)
                item.Render(writer);
        }

        void IComponentItemContainer<TComponent>.OnItemAdded(TComponent item)
        {
            this.OnItemAdded(item);
        }
    }


}
