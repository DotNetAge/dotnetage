//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Linq;

namespace DNA.Web
{
    /// <summary>
    ///  Represents an attribute that is used to handle the access permission of the Action.
    /// </summary>
    /// <remarks>
    /// When the SecurityActionAttribute specified on the Action it will be auto add a permission setting item to the Role permission settings UI.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SecurityActionAttribute : ActionFilterAttribute
    {
        private string permssionSet = "Default";
        private string title = "";
        private string description = "";
        private string redirectToAction = "";
        private bool throwOnDeny = false;
        private string resBaseName = "perms";
        private string titleResName = "";
        private string descResName = "";
        private string permssionSetResName = "";

        /// <summary>
        /// Gets/Sets the resource key for the permission set name.
        /// </summary>
        public string PermssionSetResName
        {
            get { return permssionSetResName; }
            set { permssionSetResName = value; }
        }

        /// <summary>
        /// Gets/Sets the permission description resource key.
        /// </summary>
        public string DescResName
        {
            get { return descResName; }
            set { descResName = value; }
        }

        /// <summary>
        /// Gets/Sets the permission title resource key.
        /// </summary>
        public string TitleResName
        {
            get { return titleResName; }
            set { titleResName = value; }
        }

        /// <summary>
        /// Gets/Sets the global resource base name.
        /// </summary>
        public string ResBaseName
        {
            get { return resBaseName; }
            set { resBaseName = value; }
        }

        /// <summary>
        /// Gets/Sets whether throw and exception when current user is deny access.
        /// </summary>
        public bool ThrowOnDeny
        {
            get { return throwOnDeny; }
            set { throwOnDeny = value; }
        }

        /// <summary>
        /// Gets/Sets the Action to redirect when authorize fail
        /// </summary>
        public string RedirectToAction
        {
            get
            {
                return redirectToAction;
            }
            set { redirectToAction = value; }
        }

        /// <summary>
        /// Initializes a new instance of the SecurityActionAccribute class
        /// </summary>
        /// <param name="permissionSetName">Set the PermissionSetName which this Action belongs to.</param>
        /// <param name="permissionTitle">Set the PermissionTitle of this Action</param>
        public SecurityActionAttribute(string permissionSetName, string permissionTitle)
        {
            permssionSet = permissionSetName;
            title = permissionTitle;
        }

        /// <summary>
        /// Initializes a new instance of  the SecurityActionAccribute class with permission title text
        /// </summary>
        /// <param name="permissionTitle">The permission display text of this Action</param>
        public SecurityActionAttribute(string permissionTitle)
        {
            title = permissionTitle;
        }

        /// <summary>
        /// Initializes a new instance of the SecurityActionAttribute class by specified permission set,permission title and permission description.
        /// </summary>
        /// <param name="permissionSetName">The permission set name</param>
        /// <param name="permissionTitle">The permission display title text.</param>
        /// <param name="permissionDescription">The permission description text.</param>
        public SecurityActionAttribute(string permissionSetName, string permissionTitle, string permissionDescription)
        {
            permssionSet = permissionSetName;
            title = permissionTitle;
            description = permissionDescription;
        }

        /// <summary>
        /// Get/Sets the Description of the Permission for this Action
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets/Sets the Title text of the Action
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Gets/Sets the PermissionSet name which the security action belongs to
        /// </summary>
        public string PermssionSet
        {
            get { return permssionSet; }
            set { permssionSet = value; }
        }
    }
}
