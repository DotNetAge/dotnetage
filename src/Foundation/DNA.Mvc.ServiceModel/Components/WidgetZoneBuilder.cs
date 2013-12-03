//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace DNA.Web.UI
{
    /// <summary>
    /// Represent the builder that use to build the WigetZone component.
    /// </summary>
    public class WidgetZoneBuilder : HtmlViewBuilder<WidgetZone, WidgetZoneBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the WidgetZoneBuilder class with zone and current context.
        /// </summary>
        /// <param name="zone">The WidgetZone component</param>
        /// <param name="context">The http context object.</param>
        public WidgetZoneBuilder(WidgetZone zone, HttpContextBase context) : base(zone, context) { }

        /// <summary>
        /// Define the zone content template.
        /// </summary>
        /// <param name="template">The template action </param>
        /// <returns>The WidgetZoneBuilder object</returns>
        public WidgetZoneBuilder Content(Action template)
        {
            Component.Content = template;
            return this;
        }

        /// <summary>
        /// Define the inline zone content template.
        /// </summary>
        /// <param name="template">The template function</param>
        /// <returns>The WidgetZoneBuilder object</returns>
        public WidgetZoneBuilder Content(Func<object, object> template)
        {
            Component.InlineContent = template;
            return this;
        }

        public override void Render()
        {
            Init();
            base.Render();
        }

        public override HelperResult GetHtml()
        {
            Init();
            return base.GetHtml();
        }

        /// <summary>
        /// Gets the html helper object.
        /// </summary>
        public HtmlHelper Html { get; internal set; }

        private void Init()
        {
             var context = Html.ViewContext.RequestContext.HttpContext;
            var page = App.Get().CurrentPage;
            UserDecorator user = context.Request.IsAuthenticated ? App.Get().User : null;

            if (page != null && page.Widgets != null)
            {
                var cache = Context.Cache;
                var key = "page" + page.ID + "_widgets";
                List<WidgetInstance> widgets = null;

                if (cache[key] != null)
                {
                    widgets = (List<WidgetInstance>)cache[key];
                }
                else
                {
                    widgets = page.Widgets.Where(w => w.IsExpanded).Select(w=>w.Model).ToList();
                    cache.Add(key, widgets, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                }

                var instances = widgets.Where(w => w.ZoneID.Equals(Component.Id, StringComparison.OrdinalIgnoreCase))
                                                        .Select(w => new WidgetInstanceDecorator(w, App.Get().DataContext.Widgets))
                                                        .OrderBy(w => w.ZoneID)
                                                        .ThenBy(w=>w.Pos)
                                                        .ToList();

                foreach (var instance in instances)
                {
                    if (instance.Roles != null && instance.Roles.Count() > 0)
                    {

                        if (!context.Request.IsAuthenticated) continue;
                        var inrole = false;
                        foreach (var r in instance.Roles)
                        {
                            if (user.IsInRole(r))
                            {
                                inrole = true;
                                break;
                            }
                        }
                        if (!inrole)
                            continue;
                    }

                    Component.Widgets.Add(new Widget()
                    {
                        Name = "widget_" + instance.ID.ToString(),
                        Model = instance,
                        Html = this.Html
                    });
                }
            }
        }
    }
}