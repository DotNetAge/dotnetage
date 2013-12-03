//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Xml.Solutions;
using System;
using System.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Provides the methods for template installation.
    /// </summary>
    public static class InstallationHelpers
    {
        /// <summary>
        /// Create a new web by specified web template.
        /// </summary>
        /// <param name="template">The web template object.</param>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="name">The new web name.</param>
        /// <param name="owner">The new web owner name.</param>
        /// <returns>A new web instance created from template.</returns>
        public static Web Install(this WebElement template, IDataContext dbContext, string name, string owner)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var thisWeb = dbContext.Find<Web>(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thisWeb == null)
            {
                thisWeb = new Web()
                {
                    Name = name,
                    Owner = string.IsNullOrEmpty(owner) ? name : owner,
                    Created = DateTime.UtcNow,
                    MostOnlined = DateTime.UtcNow,
                    MostOnlineUserCount = 1,
                    IsEnabled = true
                };

                //thisWeb.Popuplate(template);
                thisWeb = dbContext.Add(thisWeb);
                dbContext.SaveChanges();
            }

            foreach (var wpTmpl in template.Pages)
                wpTmpl.Install(dbContext, thisWeb);

            return thisWeb;
        }

        /// <summary>
        /// Create a new page by specified page template.
        /// </summary>
        /// <param name="pageData">The page template data.</param>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="parentWeb">The parent web that contains the new web page.</param>
        /// <param name="parentWebPageID">The parent web page id.</param>
        /// <returns>A new web page instance created from page template.</returns>
        public static WebPage Install(this PageElement pageData, IDataContext dbContext, Web parentWeb, int parentWebPageID = 0)
        {
            if (parentWeb == null)
                throw new ArgumentNullException("parentWeb");

            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            if (pageData.Title == null)
                throw new ArgumentNullException("pageData.Title");

            if (string.IsNullOrEmpty(pageData.Title.Text))
                throw new ArgumentNullException("pageData.Title.Text");

            var webName = parentWeb.Name;

            var thisPage = dbContext.WebPages.Create(parentWeb, parentWebPageID, pageData);
            dbContext.SaveChanges();

            if (parentWebPageID > 0)
            {
                var pp = dbContext.WebPages.Find(parentWebPageID);
                if (pp != null)
                    thisPage.Path = pp.Path + "/" + thisPage.ID;
            }
            else
            {
                thisPage.Path = "0/" + thisPage.ID;
            }

            if (!string.IsNullOrEmpty(thisPage.ViewData)) { 
                //Specified the view layout page
                thisPage.ViewName = string.Format("~/content/layouts/{0}/{1}.cshtml", parentWeb.Name,thisPage.ID);
            }

            dbContext.SaveChanges();

            if (pageData.Widgets != null)
            {
                if (pageData.Widgets.Count > 0)
                {
                    var groupWidgets = pageData.Widgets.GroupBy(w => w.Zone);

                    foreach (var orderedTmpl in groupWidgets)
                    {
                        var orderedWidgets = orderedTmpl.OrderBy(o => o.Sequence);
                        foreach (var widgetTmpl in orderedWidgets)
                        {
                            try
                            {
                                widgetTmpl.Install(dbContext, thisPage);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("There is an error occur in adding the widget \"" + widgetTmpl.Title.Text + "\" to \"" + pageData.Title.Text + "\" page", e);
                            }
                        }
                    }
                    dbContext.SaveChanges();
                }
            }

            if (pageData.Children != null)
            {
                foreach (var child in pageData.Children)
                {
                    try
                    {
                        child.Install(dbContext, parentWeb, thisPage.ID);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("There is an error occur in creating the \"" + child.Title.Text + "\" page", e);
                    }
                }
            }

            return thisPage;
        }

        /// <summary>
        /// Create new widget instance by specified widget descriptor.
        /// </summary>
        /// <param name="descriptor">The widget descriptor object.</param>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="parentPage">The parent web page object.</param>
        /// <param name="zoneID">The zone element id which contains the new widget.</param>
        /// <param name="pos">The position of the new widget.</param>
        /// <returns>A widget instance created by specified widget descriptor.</returns>
        public static WidgetInstance Install(this WidgetDescriptor descriptor, IDataContext dbContext, WebPage parentPage, string zoneID = "zone0", int pos = 0)
        {
            return dbContext.Widgets.AddWidget(descriptor, parentPage.ID, zoneID, pos);
        }

        /// <summary>
        /// Create new widget insance by specified widget data.
        /// </summary>
        /// <param name="widgetTemplate">The widget data.</param>
        /// <param name="dbContext">The data context object.</param>
        /// <param name="parentPage">The web page instanct which contains the new widget.</param>
        /// <returns>A new widget instance created by specified widget data.</returns>
        public static WidgetInstance Install(this IWidget widgetTemplate, IDataContext dbContext, WebPage parentPage)
        {
            return dbContext.Widgets.AddWidget(widgetTemplate, parentPage.ID, string.IsNullOrEmpty(widgetTemplate.ZoneID) ? "zone0" : widgetTemplate.ZoneID, widgetTemplate.Pos);
        }
    }
}
