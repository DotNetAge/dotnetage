///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DNA.Web
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class PagableAttribute : FilterAttribute, IActionFilter
    {
        public PagableAttribute() { }

        private bool allowJsonget=true;

        public bool AllowJsonGet
        {
            get { return allowJsonget; }
            set { allowJsonget = value; }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var wrapper = filterContext.Controller.ViewData.Model as IModelWrapper;
            if (wrapper == null) return;

            if ((filterContext.HttpContext.Request.IsAjaxRequest()) && AllowJsonGet)
            {     
                if ((wrapper.Model as IEnumerable<DynamicGroupResult>) != null)
                ((ModelWrapper)wrapper).Model=ModelBinder.ConvertDataResult(wrapper.Model as IEnumerable<DynamicGroupResult>);

                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = wrapper
                };
            }
            else
            {
                filterContext.Controller.ViewData["totalRecords"] = wrapper.Total;
                filterContext.Controller.ViewData.Model = wrapper.Model;
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}
