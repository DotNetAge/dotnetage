//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.UI;
using DNA.Xml.Solutions;
using DNA.Xml.Widgets;
using System;
using System.Linq;

namespace DNA.Web
{
    public static class JSONExtensions
    {

        public static dynamic ToJSON(this WidgetPackage pkg, IWidgetDescriptorRepository repository)
        {
            var widget = pkg.Model;
            var Url = UrlUtility.CreateUrlHelper();
            //var repository = WebSiteContext.Current.DataContext.WidgetDescriptors;
            var installPath = pkg.Category + "\\" + pkg.Name;
            return new
                   {
                       id = pkg.Name,
                       category=pkg.Category,
                       uid = widget.ID,
                       name = new
                       {
                           shortname = widget.Name != null ? widget.Name.ShortName : "Unknown",
                           fullname = widget.Name != null ? widget.Name.FullName : "Unknown"
                       },
                       icons = widget.Icons != null ? (widget.Icons.Select(i => new
                       {
                           src = Url.Content(pkg.ResolveUri(i.Source)),
                           width = i.Width,
                           height = i.Height
                       })) : null,
                       desc = GE.GetContent(widget.Description),
                       author = widget.Author != null ? new //The user object is read from remote
                       {
                           name = widget.Author.Name,
                           href = widget.Author.Uri,
                           email = widget.Author.Email
                       } : null,
                       totalUses = repository.InusingWidgetsCount(installPath),
                       ratings = 0,// from remote
                       installed = repository.Contains(w => w.UID.Equals(widget.ID, StringComparison.OrdinalIgnoreCase)),
                       version = widget.Version,
                       languages = pkg.GetSupportLanguages(),
                       //signed = pkg.HasSigned,
                       //verified = pkg.Verify(),
                       licenses = widget.Licenses != null ? (widget.Licenses.Select(l => new
                       {
                           name = l.Text,
                           href = l.Source
                       })) : null
                       //modified = tmpl.Modified
                   };
        }

        public static dynamic ToJSON(this PageElement tmpl)
        {
            return new
            {
                name = tmpl.Name,
                title = tmpl.Title != null ? GE.GetContent(tmpl.Title.Text) : "",
                desc = tmpl.Description != null ? GE.GetContent(tmpl.Description.Text) : "",
                icon = tmpl.Icon,
                picture = tmpl.Image
            };
        }

        //public static dynamic ToJSON(this PackageElement package)
        //{
        //    return new
        //    {
        //        name = package.ID,
        //        title = package.Title != null ? package.Title.Text : package.ID,
        //        desc =package.Description!=null ? package.Description.Text : "",
        //        version = package.Version,
        //        published = package.Published,
        //        //author =null,
        //        licenses = package.Licenses.Select(l => new
        //        {
        //            title = l.Text,
        //            url = l.Source
        //        }),
        //        images = package.Images.Select(i => new
        //        {
        //            title = i.Title,
        //            url = i.Source //, //package.ResolveUrl(i.Url),
        //            //width = i.Width,
        //            //height = i.Height
        //        }),
        //        links = package.Links.Select(l => new
        //        {
        //            title = l.Title,
        //            url = l.NavigateUrl
        //        })
        //    };
        //}

        //public static string GeneratePermalink(this WebPage page, string webName, IWebPageRepository repository)
        //{
        //    string path = TextUtility.Slug(page.Title);
        //    if ((string.IsNullOrEmpty(webName)) || webName.Equals("home", StringComparison.OrdinalIgnoreCase))
        //        path = "~/sites/home/" + path;
        //    else
        //        path = "~/sites/" + webName.ToLower() + "/" + path;

        //    if (!repository.IsExists(path.ToLower() + ".html"))
        //        path += ".html";
        //    else
        //        path += "-" + DateTime.Now.ToString("yyMMddHHmm") + ".html";
        //    page.Path = path;
        //    return path;
        //}

        public static string CurrentWebName(this System.Web.Mvc.Controller ctrl)
        {
            var website = "home";
            if (ctrl.RouteData.Values.ContainsKey("website"))
                website = ctrl.RouteData.Values["website"] as string;
            return website;
        }
        //public static dynamic ToJSON(this WebPage page)
        //{
        //    return new
        //    {
        //        id = page.ID,
        //        isDefault = false, ///TODO Get default
        //        isMenu = page.ShowInMenu,
        //        isSitemap = page.ShowInSitemap,
        //        linkto = page.LinkTo,
        //        title = GE.GetContent(page.Title),
        //        desc = GE.GetContent(page.Description),
        //        keywords = GE.GetContent(page.Keywords),
        //        icon = page.IconUrl,
        //        picture = page.ImageUrl,
        //        parentId = page.ParentID,
        //        target = !string.IsNullOrEmpty(page.Target) ? page.Target : "_self",
        //        shared = page.IsShared,
        //        @static = page.IsStatic,
        //        layout = page.ViewName,
        //        modified = page.LastModified,
        //        path = VirtualPathUtility.ToAbsolute(page.Path),
        //        roles = page.Roles != null ? page.Roles.Select(r => r.RoleName).ToArray() : null,
        //        allowAnonymous = page.AllowAnonymous
        //    };
        //}
        //private static JavaScriptSerializer serializer;
        //static JSONExtensions() { serializer = new JavaScriptSerializer(); }

        //static string Encode(object instance) 
        //{
        //    if (instance != null)
        //        return serializer.Serialize(instance);
        //    return "";
        //}

        //static dynamic Decode(string source)
        //{ 
        //    if (!string.IsNullOrEmpty(source))
        //        return serializer.Deserialize(
        //}

        //public static dynamic ToJSON(this WidgetDescriptor descriptor, string website = "")
        //{
        //    var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
        //    var contentUrl = "";
        //    if (!string.IsNullOrEmpty(descriptor.Controller) && !string.IsNullOrEmpty(descriptor.Action))
        //        contentUrl = !string.IsNullOrEmpty(website) ? Url.Action(descriptor.Action, descriptor.ControllerShortName, new { Area = string.IsNullOrEmpty(descriptor.Area) ? "" : descriptor.Area, website = website, id = descriptor.ID, preview = true }) : Url.Action(descriptor.Action, descriptor.ControllerShortName, new { Area = string.IsNullOrEmpty(descriptor.Area) ? "" : descriptor.Area, id = descriptor.ID, preview = true });
        //    else
        //        contentUrl = !string.IsNullOrEmpty(website) ? Url.Action("Generic", "Widget", new { Area = "", website = website, id = descriptor.ID, preview = true }) : Url.Action("Generic", "Widget", new { Area = "", id = descriptor.ID, preview = true });
        //    // var _defaults = !string.IsNullOrEmpty(descriptor.Defaults) ? Json.Decode(descriptor.Defaults) : null;

        //    return new
        //    {
        //        id = descriptor.ID,
        //        title = GE.GetContent(descriptor.Title),
        //        description = GE.GetContent(descriptor.Description),
        //        icon = string.IsNullOrEmpty(descriptor.IconUrl) ? null : Url.Content(descriptor.IconUrl),
        //        contentUrl = contentUrl,
        //        contentType = descriptor.ContentType,
        //        version = descriptor.Version,
        //        defaults = descriptor.Defaults
        //    };
        //}
    }
}