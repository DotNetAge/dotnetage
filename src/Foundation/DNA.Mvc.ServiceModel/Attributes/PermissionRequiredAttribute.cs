//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


using System;
using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    ///  Represents an attribute that is used to inherit the permission form specifed Controller and Action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PermissionRequiredAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets/Sets the Controller which the permission inherit from.
        /// </summary>
        /// <remarks>
        /// When this property set that means current Action has the same permission with the specified Controller.
        /// </remarks>
        public Type ControllerType { get; set; }

        /// <summary>
        /// Gets/Sets the controller type name instead to specfied the strongly type on ControllerType property
        /// </summary>
        public string ControllerTypeName { get; set; }

        /// <summary>
        /// Gets/Sets the ActionName which specified SecurityActionAttribute
        /// </summary>
        /// <remarks>
        /// When this property and the ControllerType set that means current Action has the same permission with the specified Action.
        /// </remarks>
        public string ActionName { get; set; }

        /// <summary>
        /// Initializes a new instance of  the PermissionRequiredAttribute class with specifed action name.
        /// </summary>
        /// <param name="action">The Action name of current controller</param>
        public PermissionRequiredAttribute(string action) { this.ActionName = action; }

        /// <summary>
        ///  Initializes a new instance of the PermissionRequiredAttribute class with specifed controller type and action name.
        /// </summary>
        /// <param name="controller">The controller which defined the action</param>
        /// <param name="action">The action which has SecurityActionAttribute defined.</param>
        public PermissionRequiredAttribute(Type controller, string action) : this(action) { this.ControllerType = controller; }

        ///// <summary>
        /////  Initializes a new instance of the PermissionRequiredAttribute class with specifed controller type name string and action name.
        ///// </summary>
        ///// <param name="contrllerType">The controller type</param>
        ///// <param name="action">The action name.</param>
        //public PermissionRequiredAttribute(string contrllerType, string action) : this(action) { this.ControllerTypeName = contrllerType; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.ControllerType == null)
            {
                if (!string.IsNullOrEmpty(this.ControllerTypeName))
                    ControllerType = Type.GetType(this.ControllerTypeName);
                ControllerType = filterContext.Controller.GetType();
            }

            if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated && App.Get().Context.HasPermisson(ControllerType, ActionName))
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new HttpUnauthorizedResult();

        }
    }
}
