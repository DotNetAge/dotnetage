//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace DNA.Web.Data.Entity
{
    public class WidgetRepository : EntityRepository<WidgetInstance>, IWidgetRepository
    {
        public WidgetRepository() : base() { }

        public WidgetRepository(CoreDbContext dbContext) : base(dbContext) { }

        /// <summary>
        /// Create the Widget Instance by specified widget descriptor ,the web page visual path ,the zone element id and the position in widgets sequence.
        /// </summary>
        /// <param name="id">The new widget id.</param>
        ///<param name="descriptor">The WidgetDescriptor instance.</param>
        /// <param name="webPagePath">Specified the web page path to added</param>
        /// <param name="zoneID">The zone element id</param>
        /// <param name="position"> The position in widgets sequence</param>
        /// <returns>A new Widget instance.</returns>
        public virtual WidgetInstance AddWidget(WidgetDescriptor descriptor, int pageID, string zoneID, int position)
        {
            int pos = 0;
            if (position > -1)
                pos = position;

            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            if (pageID == 0)
                throw new ArgumentNullException("pageID");

            if (string.IsNullOrEmpty(zoneID))
                throw new ArgumentNullException(zoneID);

            WebPage page = Context.WebPages.Find(pageID);

            if (page == null)
                throw new WebPageNotFoundException();

            var widgetsForUpdate = DbSet.Where(w => w.PageID == page.ID && (w.ZoneID == zoneID))
                                                                 .OrderBy(w => w.Pos)
                                                                 .ToList();

            if (widgetsForUpdate != null)
            {
                for (var i = 0; i < widgetsForUpdate.Count; i++)
                {
                    var _w = widgetsForUpdate[i];
                    _w.Pos = i;
                    if (i >= pos) 
                        _w.Pos = i + 1;
                }

                if (IsOwnContext)
                    Context.SaveChanges();
            }

            WidgetInstance widget = new WidgetInstance()
            {
                //ID = id,
                IsExpanded = true,
                Pos = pos,
                ZoneID = zoneID
            };

            ///TODO:We need to load the localization contents from package which descriptor refers to.
            //widget.IconUrl = descriptor.IconUrl;
            widget.Title = descriptor.Title;
            widget.Locale = page.Locale;
            widget.Data = descriptor.Defaults;
            widget.ViewMode = descriptor.ViewModes;
            widget.ShowHeader = true;
            widget.ShowBorder = true;
            widget.IsExpanded = true;
            widget.WidgetDescriptor = descriptor;
            widget.WebPage = page;
            widget.CssText = "";
            //widget.Cached = descriptor.CacheEnabled;
            if (!descriptor.DefaultLocale.Equals(page.Locale))
            {
                var locName = descriptor.LocaleName(page.Locale);
                if (!string.IsNullOrEmpty(locName.FullName))
                    widget.Title = locName.FullName;
                //var locDesc=descriptor.LocaleDesc(page.Locale);
                //if (!string.IsNullOrEmpty(locDesc))
                //    widget.
            }

            if (descriptor.Height > 0)
                widget.BodyCssText = "height:" + descriptor.Height.ToString() + "px;";

            if (descriptor.ViewModes == "floating" && descriptor.Width > 0)
            {
                widget.CssText += "position:absolute;top:0px;left:0px;";
                widget.BodyCssText = "width" + descriptor.Width + "px;";
            }

            try
            {
                if (!string.IsNullOrEmpty(descriptor.Defaults))
                {
                    var serializer = new JavaScriptSerializer();
                    var defaults = serializer.Deserialize<List<Dictionary<string, object>>>(descriptor.Defaults);
                    foreach (var def in defaults)
                    {
                        var name = def["name"].ToString();
                        if (name.Equals("showHeader", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("sys_header", StringComparison.OrdinalIgnoreCase))
                            widget.ShowHeader = (bool)def["value"];

                        if (name.Equals("showBorder", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("sys_border", StringComparison.OrdinalIgnoreCase))
                            widget.ShowBorder = (bool)def["value"];

                        if (name.Equals("transparent", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("sys_tran", StringComparison.OrdinalIgnoreCase))
                            widget.Transparent = (bool)def["value"];

                        if (name.Equals("cached", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("sys_cached", StringComparison.OrdinalIgnoreCase))
                            widget.Cached = (bool)def["value"];

                        if (name.Equals("showIn", StringComparison.OrdinalIgnoreCase) ||
                            name.Equals("sys_shows", StringComparison.OrdinalIgnoreCase))
                            widget.ShowMode = (int)def["value"];

                        //if (name.Equals("sys_height", StringComparison.OrdinalIgnoreCase))
                        //widget.Height = (int)def["value"];

                        //if (name.Equals("sys_width", StringComparison.OrdinalIgnoreCase))
                        //widget.Width = (int)def["value"];

                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Deserialize " + descriptor.Title + " widget default values error.The default value definition is not correct. Please check your widget config.xml.");
            }

            Create(widget);

            return widget;
        }

        /// <summary>
        /// Create the Widget instance by specified widget template.
        /// </summary>
        /// <param name="id">The new widget id.</param>
        /// <param name="widgetData">The widget template object.</param>
        /// <param name="webPagePath">The virutal path of the web page which the widget add to.</param>
        /// <param name="zoneID">The widget zone id that contains the new widget.The Widget will be added to zone0 when this paramater set to null </param>
        /// <param name="position">The widget position.</param>
        /// <returns>Returns a new widget instance.</returns>
        public virtual WidgetInstance AddWidget(IWidget widgetData, int pageID, string zoneID = null, int position = 0)
        {
            var descriptor = Context.WidgetDescriptors.FirstOrDefault(w => w.InstalledPath.Equals(widgetData.UID, StringComparison.OrdinalIgnoreCase) || w.UID.Equals(widgetData.UID, StringComparison.OrdinalIgnoreCase));
            if (descriptor == null)
                throw new Exception(string.Format("Widget template \"{0}\" not registered", widgetData.UID));

            var _zoneID = "zone0";

            if (string.IsNullOrEmpty(zoneID))
            {
                if (!string.IsNullOrEmpty(widgetData.ZoneID))
                    _zoneID = widgetData.ZoneID;
            }
            else
                _zoneID = zoneID;

            var widget = AddWidget(descriptor, pageID, _zoneID, position);

            widget.Popuple(widgetData);

            //if (string.IsNullOrEmpty(widget.IconUrl) && !string.IsNullOrEmpty(descriptor.IconUrl))
            //    widget.IconUrl = descriptor.IconUrl;

            if (!string.IsNullOrEmpty(widget.Data))
            {
                var properties = widget.ReadUserPreferences();
                foreach (var def in properties)
                {
                    var name = def["name"].ToString();
                    if (name.Equals("showHeader", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("sys_header", StringComparison.OrdinalIgnoreCase))
                        widget.ShowHeader = (bool)def["value"];

                    if (name.Equals("showBorder", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("sys_border", StringComparison.OrdinalIgnoreCase))
                        widget.ShowBorder = (bool)def["value"];

                    if (name.Equals("transparent", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("sys_tran", StringComparison.OrdinalIgnoreCase))
                        widget.Transparent = (bool)def["value"];

                    if (name.Equals("cached", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("sys_cached", StringComparison.OrdinalIgnoreCase))
                        widget.Cached = (bool)def["value"];

                    if (name.Equals("showIn", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("sys_shows", StringComparison.OrdinalIgnoreCase))
                        widget.ShowMode = (int)def["value"];
                }
            }


            if (IsOwnContext)
                Context.SaveChanges();

            return widget;
        }

        /// <summary>
        /// Move the specified widget to a zone.
        /// </summary>
        /// <param name="id">The widget instance id.</param>
        /// <param name="zoneID">the zone element id</param>
        /// <param name="position">The position in widgets sequence</param>
        public virtual void MoveTo(int id, string zoneID, int position)
        {
            var target = Find(id);

            if (target == null)
                throw new Exception("Widget not found.");

            var _pageID = target.PageID;

            var widgets = DbSet.Where(w => w.PageID == _pageID);

            var seqCollection = widgets.Where(w => w.ZoneID.Equals(zoneID, StringComparison.OrdinalIgnoreCase))
                                                .OrderBy(p => p.Pos)
                                                .Select(p => p.Pos);

            if (seqCollection.Count() == 0)
            {
                target.Pos = 0;
                target.ZoneID = zoneID;
                if (IsOwnContext)
                    Context.SaveChanges();
                return;
            }

            int upperBound = seqCollection.Max();
            int lowerBound = seqCollection.Min();

            if (upperBound == 0 && lowerBound == 0)
                upperBound = seqCollection.Count();

            int _from = target.Pos;
            int _to = position;

            if (_to > upperBound)
                _to = upperBound;
            else
            {
                if (_to < lowerBound)
                    _to = lowerBound;
            }

            //1.Move up
            if (_from > _to)
            {
                widgets.Where(w => w.ZoneID.Equals(zoneID, StringComparison.OrdinalIgnoreCase) && w.Pos >= _to && w.Pos < _from)
                    .OrderBy(p => position)
                    .AsParallel()
                    .ForAll(p => p.Pos++);

            }

            //2.Move down
            if (_from < _to)
            {
                widgets.Where(w => w.ZoneID.Equals(zoneID, StringComparison.OrdinalIgnoreCase) && w.Pos > _from && w.Pos <= _to)
                   .OrderBy(p => position)
                   .AsParallel()
                   .ForAll(p => p.Pos--);
            }

            target.Pos = _to;
            target.ZoneID = zoneID;

            if (IsOwnContext)
                Context.SaveChanges();

        }

        public void AddRoles(int id, string[] roles)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("id");

            var widget = Find(id);

            if (widget == null)
                throw new Exception("Widget not found");

            widget.Roles.Clear();
            if (roles != null && roles.Length > 0)
            {
                var rs = context.Roles.Where(r => roles.Contains(r.Name)).ToList();
                foreach (var r in rs)
                    widget.Roles.Add(r);
            }

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public string[] GetRoles(int id)
        {
            if (id == 0)
                throw new ArgumentException("id");

            var widget = context.Widgets.Find(id);
            if (widget == null)
                throw new Exception("Widget not found");

            if (widget.Roles != null)
                return widget.Roles.Select(r => r.Name).ToArray();
            else
                return new string[0];
        }
    }
}
