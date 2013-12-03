//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    /// The filter class use to read the ContentList,ContentItem and ContentForm instance for current context.
    /// </summary>
    /// <remarks>
    /// This filter will generate the instances to App.Context and auto create the form page when it's not exists.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ContentFormPageAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets/Sets the ContentForm type
        /// </summary>
        public ContentFormTypes FormType { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var app = App.Get();
            var ctx = app.Context;
            var name = (string)filterContext.RouteData.Values["list_name"];
            var slug = (string)filterContext.RouteData.Values["item_slug"];

            var urlformat = "lists/{0}/items/id"; //default format for detail form
            var culture = ctx.Locale;
            var ver = 0;

            #region get form
            if (!string.IsNullOrEmpty(name))
            {
                ctx.List = app.CurrentWeb.Lists[name];
                switch (FormType)
                {
                    case ContentFormTypes.New:
                        ctx.Form = ctx.List.NewForm;
                        urlformat = "lists/{0}/new";
                        break;
                    case ContentFormTypes.Edit:
                        ctx.Form = ctx.List.EditForm;
                        urlformat = "lists/{0}/edit/id";
                        break;
                    default:
                        ctx.Form = ctx.List.DetailForm;
                        break;
                }

                //check roles
                if (ctx.Form != null && !ctx.Form.IsAuthorized(filterContext.RequestContext.HttpContext))
                {
                    filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
                    return;
                }
            }
            #endregion

            #region get Item
            var dynamicPageTitle = "";

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(slug))
            {
                var strVer = (string)filterContext.RequestContext.HttpContext.Request.QueryString["ver"];
                if (!string.IsNullOrEmpty(strVer))
                    int.TryParse(strVer, out ver);
                ctx.DataItem = ctx.List.GetItem(slug, ver);

                if (ctx.DataItem == null)
                {
                    //Try to find data item by id
                    var itemGid = Guid.Empty;
                    if (Guid.TryParse(slug, out itemGid))
                    {
                        var tryItem = ctx.List.GetItem(itemGid);
                        if (ctx.List.EnableVersioning && !string.IsNullOrEmpty(strVer))
                        {
                            ctx.DataItem = ctx.List.GetItem(tryItem.Slug, ver);
                        }
                        else ctx.DataItem = tryItem;
                    }
                }

                if (ctx.DataItem != null && ctx.Form.FormType == (int)ContentFormTypes.Display && ctx.List.IsModerated)
                {
                    #region  Check moderate item
                    var httpCtx = filterContext.HttpContext;
                    var userName =httpCtx.Request.IsAuthenticated ? httpCtx.User.Identity.Name : "";
                    
                   
                    if (ctx.DataItem.ModerateState != (int)ModerateStates.Approved)
                    {
                        if (string.IsNullOrEmpty(userName) || (!ctx.DataItem.IsOwner(userName) && !ctx.List.IsModerator(userName) && !ctx.List.IsOwner(userName)))
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                            return;
                        }
                    }
                    #endregion
                }

                var defaultTitleField = ctx.List.GetDefaultTitleField();
                if (defaultTitleField != null)
                {
                    if (ctx.DataItem != null)
                    {
                        var titleRaw = ctx.DataItem.Value(defaultTitleField.Name).Raw;
                        dynamicPageTitle = titleRaw != null ? titleRaw.ToString() : "";
                        filterContext.Controller.ViewBag.Title = dynamicPageTitle;
                    }
                }
            }

            #endregion

            #region get Page
            if (!filterContext.HttpContext.Request.IsAjaxRequest() && !filterContext.IsChildAction)
            {
                var slugFormatted = string.Format(urlformat, name);
                var page = app.CurrentWeb.FindPage(culture, slugFormatted);

                if (page == null)
                {
                    var defaultView = ctx.List.Views.Default;
                    var parentPageID = 0;
                    var parentPath = "";
                    if (defaultView != null)
                    {
                        var parentSlug = string.Format("lists/{0}/views/{1}", ctx.List.Name, defaultView.Name);
                        var parentPage = app.CurrentWeb.FindPage(culture, parentSlug);
                        if (parentPage != null)
                        {
                            parentPageID = parentPage.ID;
                            parentPath = parentPage.Path;
                        }
                    }

                    var newpage = new WebPage()
                    {
                        Slug = slugFormatted,
                        IsShared = true,
                        IsStatic = true,
                        Title = string.IsNullOrEmpty(ctx.Form.Title) ? ((ContentFormTypes)ctx.Form.FormType).ToString() : ctx.Form.Title,
                        ParentID = parentPageID,
                        Description = ctx.Form.Description,
                        ShowInMenu = false,
                        ShowInSitemap = false,
                        AllowAnonymous = true,
                        Created = DateTime.Now,
                        LastModified = DateTime.Now,
                        WebID = app.CurrentWeb.Id,
                        Owner = app.CurrentWeb.Owner,
                        Dir = app.CurrentWeb.Dir,
                        Locale = culture
                    };

                    app.DataContext.WebPages.Create(newpage);
                    app.DataContext.SaveChanges();
                    newpage.Path = (string.IsNullOrEmpty(parentPath) ? "0" : parentPath) + "/" + newpage.ID.ToString();
                    app.DataContext.SaveChanges();
                    page = app.Wrap(newpage);

                    #region create static widgets
                    //1.create view widget
                    var formDescriptor = app.Descriptors[FormType == ContentFormTypes.New ? @"content\newform" : @"content\dataform"];
                    var formWidget = app.AddWidgetTo(app.CurrentWeb.Name, page.ID, formDescriptor.ID, "ContentZone", 1);
                    formWidget.ShowHeader = false;
                    formWidget.ShowBorder = false;
                    formWidget.Title = ctx.Form.Title;

                    if (FormType != ContentFormTypes.New)
                    {
                        formWidget.SetUserPreference("slug", slug);
                        formWidget.SetUserPreference("type", (int)FormType);
                    }

                    formWidget.SetUserPreference("listName", ctx.List.Name);

                    if (FormType == ContentFormTypes.Display)
                    {
                        formWidget.SetUserPreference("showDetail", false);
                        formWidget.SetUserPreference("showAttachs", true);
                        formWidget.SetUserPreference("showTags", true);
                        formWidget.SetUserPreference("showComments", true);
                        formWidget.SetUserPreference("showVersions", true);
                        //formWidget.SetUserPreference("showCats", true);
                        formWidget.SetUserPreference("showVotes", true);
                    }

                    formWidget.IsStatic = true;
                    formWidget.Save();

                    #endregion

                    if (ctx.Form.Roles != null && ctx.Form.Roles.Count() > 0)
                        page.AddRoles(ctx.Form.Roles);
                    ctx.Page = newpage;
                }
                else
                {
                    ctx.Page = page;
                }

                var pageWrapper = app.Wrap(page);
                if (!string.IsNullOrEmpty(dynamicPageTitle))
                    pageWrapper.Title = dynamicPageTitle;

                filterContext.Controller.ViewBag.Page = pageWrapper;
            }
            #endregion

            base.OnActionExecuting(filterContext);
        }
    }
}
