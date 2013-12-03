using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace DNA.Mvc
{
    public class ListViewComponent<TItemComponent, TModel> : ContainerViewComponent<TItemComponent>
        where TItemComponent : ListItemViewComponent<TModel>
        where TModel : class
    {
        public override string TagName
        {
            get
            {
                return "ul";
            }
        }
    }

    public class ListItemViewComponent<TModel> : HtmlComponent
    {
    }
}
