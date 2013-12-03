//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNA.Xml.Solutions;
using DNA.Utility;

namespace DNA.Web.Data.Entity
{
    public class WebPageRepository : EntityRepository<WebPage>, IWebPageRepository
    {
        public WebPageRepository() : base() { }

        public WebPageRepository(CoreDbContext dbContext) : base(dbContext) { }

        /// <summary>
        /// Delete the page by specified page id
        /// </summary>
        /// <param name="id">The web page id.</param>
        public virtual void Delete(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");

            this.Context.Database.ExecuteSqlCommand(string.Format("DELETE FROM dna_WebPages WHERE ID={0}", id));
        }

        /// <summary>
        /// Find web page by virual path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual WebPage Find(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return this.DbSet.SqlQuery(string.Format("SELECT * FROM dna_WebPages WHERE [Path]=N'{0}'", path.ToLower())).SingleOrDefault();
        }

        public virtual string[] GetRoles(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            var page = Context.WebPages.Find(id);
            if (page.Roles != null)
                return page.Roles.Select(r => r.Name).ToArray();
            else
                return null;
            //return Context.WebPageRoles.Where(r => r.PageID == id).Select(p => p.RoleName).ToArray();
        }

        public virtual IEnumerable<WebPage> GetChildren(int webID, int parentID)
        {
            if (parentID < 0) throw new ArgumentOutOfRangeException("parentID");

            return this.DbSet.AsNoTracking().Where(p => p.ParentID == parentID && p.WebID == webID).ToList();
        }

        //public virtual bool IsExists(string path)
        //{
        //    if (string.IsNullOrEmpty(path)) return false;
        //    return DbSet.Count(p => p.Path.Equals(path, StringComparison.OrdinalIgnoreCase)) > 0;
        //}

        //private void Reindex(IEnumerable<WebPage> webpages, int parentID, string locale)
        //{
        //    //var pages = webpages.Where(p => p.ParentID == parentID && p.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase)).ToList();
        //    for (int i = 0; i < webpages.Count; i++)
        //        webpages[i].Pos = i;
        //}

        public virtual void Move(int parentID, int id, int pos)
        {
            var page = Find(id);

            if (page == null)
                throw new WebPageNotFoundException(id.ToString());

            var webPages = DbSet.Where(p => p.WebID == page.WebID);
            var seqCollection = webPages.Where(p => p.ParentID == parentID)
                                                .OrderBy(p => p.Pos)
                                                .Select(p => p.Pos);

            if (seqCollection.Count() == 0)
            {
                page.Pos = 0;
                page.ParentID = parentID;

                if (parentID > 0)
                {
                    var parentPage = dbSet.Find(parentID);
                    if (parentPage != null)
                        page.Path = parentPage.Path + "/" + page.ID.ToString();
                    else
                    {
                        if (dbSet.Find(parentID) == null)
                            throw new WebPageNotFoundException(string.Format("WebPageRepositiory.Move invoke fail. Parent page not found.Here is not page id={0}", parentID));
                    }
                }
                else
                    page.Path = "0/" + page.ID.ToString();

                if (IsOwnContext)
                    Context.SaveChanges();
                return;
            }

            int upperBound = seqCollection.Max();
            int lowerBound = seqCollection.Min();

            if (upperBound == 0 && lowerBound == 0)
                upperBound = seqCollection.Count();

            int _from = page.Pos;
            int _to = pos;

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
                webPages.Where(p => p.ParentID == parentID && p.Pos >= _to && p.Pos < _from)
                    .OrderBy(p => pos)
                    .AsParallel()
                    .ForAll(p => p.Pos++);

            }

            //2.Move down
            if (_from < _to)
            {
                webPages.Where(p => p.ParentID == parentID && p.Pos > _from && p.Pos <= _to)
                   .OrderBy(p => pos)
                   .AsParallel()
                   .ForAll(p => p.Pos--);
            }

            page.Pos = _to;
            page.ParentID = parentID;

            if (parentID > 0)
            {
                var parentPage = dbSet.Find(parentID);
                if (parentPage != null)
                    page.Path = parentPage.Path + "/" + page.ID.ToString();
            }
            else
                page.Path = "0/" + page.ID.ToString();

            //this.Reindex(dbSet.Where(p => p.ParentID == page.ParentID));
            //var siblings = dbSet.Where(p => p.ParentID == page.ParentID && p.Locale.Equals(page.Locale,StringComparison.OrdinalIgnoreCase) && p.WebID==page.WebID).OrderBy(p=>p.Pos).ToList();
            //for (int i = 0; i < siblings.Count; i++)
            //    siblings[i].Pos = i;

            if (IsOwnContext) Context.SaveChanges();
        }

        public override void Delete(WebPage t)
        {
            if (t == null) throw new ArgumentNullException("t");

            if (t.ID <= 0) throw new ArgumentOutOfRangeException("Web.ID");

            Context.Database.ExecuteSqlCommand(string.Format("UPDATE dna_WebPages SET ParentID={0} WHERE ParentID={1}", t.ParentID, t.ID));
            //Context.
            base.Delete(t);
        }

        public override WebPage Update(WebPage TObject)
        {
            TObject.LastModified = DateTime.Now;
            return base.Update(TObject);
        }

        public override WebPage Create(WebPage t)
        {
            if (t.Web == null && t.WebID == 0)
                throw new ArgumentNullException("The \"Web\" property can not be null.");

            if (string.IsNullOrEmpty(t.Slug))
                throw new ArgumentNullException("The \"Slug\" property can not be empty.");

            if (string.IsNullOrEmpty(t.Owner))
                throw new ArgumentNullException("The \"Owner\" property can not be null.");

            if (DbSet.Count(p => p.Slug.Equals(t.Slug, StringComparison.OrdinalIgnoreCase) && p.Locale.Equals(t.Locale) && p.WebID.Equals(t.WebID)) > 0)
                throw new WebPageIsExistsException(string.Format("{0}.html", t.Slug));

            var webid = t.Web == null ? t.WebID : t.Web.Id;
            if (string.IsNullOrEmpty(t.Target))
                t.Target = "_self";

            if (t.Version == 0)
                t.Version = 1;

            if ((t.Created == null) || (t.Created.Equals(DateTime.MinValue)))
                t.Created = DateTime.UtcNow;

            if ((t.LastModified == null) || (t.LastModified.Equals(DateTime.MinValue)))
                t.LastModified = DateTime.UtcNow;

            var seq = DbSet.Where(p => p.ParentID == t.ParentID && p.WebID == webid).OrderBy(p => p.Pos).Select(p => p.Pos);
            if (seq.Count() > 0)
                t.Pos = seq.Max() + 1;

            return base.Create(t);
        }

        public virtual WebPage Create(Web web, int parentID, IWebPage template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            var newPage = new WebPage()
            {
                ParentID = parentID,
                Owner = web.Owner,
                Web = web,
                Version = 1
            };

            newPage.Populate(template);
            var slug = newPage.Slug;
            var slugs = DbSet.Where(p => p.Locale.Equals(newPage.Locale, StringComparison.OrdinalIgnoreCase) && p.WebID.Equals(web.Id)).Select(p => p.Slug).ToArray();
            if (string.IsNullOrEmpty(slug))
            {
                if (string.IsNullOrEmpty(newPage.Title))
                    throw new Exception("The Title property or Slug property must be set.");

                slug = TextUtility.Slug(newPage.Title);
                newPage.Slug = slug;
            }
            var i = 0;
            while (slugs.Contains(newPage.Slug))
                newPage.Slug = slug + (++i).ToString();

            if (string.IsNullOrEmpty(newPage.Slug))
                throw new ArgumentNullException("The Slug property of the new web page must be set.");

            if (parentID > 0)
            {
                if (dbSet.Find(parentID) == null)
                    throw new WebPageNotFoundException(string.Format("WebPageRepositiory.Create invoke fail. Parent page not found.Here is not page id={0}", parentID));
            }

            return this.Create(newPage);
        }

        public virtual void AddRoles(WebPage page, string[] roles)
        {
            if (page == null) throw new ArgumentNullException("page");

            if (page.ID <= 0) throw new WebPageNotFoundException();

            if ((roles == null) || (roles.Length == 0)) throw new ArgumentNullException("roles");

            var rs = context.Roles.Where(r => roles.Contains(r.Name)).ToList();

            if (page.Roles != null)
                page.Roles.Clear();

            if (rs.Count > 0 && page.Roles == null)
                page.Roles = new List<Role>();

            foreach (var r in rs)
                page.Roles.Add(r);

            if (page.AllowAnonymous)
                page.AllowAnonymous = false;

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public virtual void AddRoles(int id, string[] roles)
        {
            if (id == 0) throw new ArgumentOutOfRangeException("id");
            if ((roles == null) || (roles.Length == 0)) throw new ArgumentNullException("roles");
            var page = this.Find(id);
            this.AddRoles(page, roles);
        }

        public virtual void ClearRoles(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException("id");
            var rows = Context.Database.ExecuteSqlCommand(string.Format("DELETE FROM dna_WebPageRoles WHERE PageID={0}", id));
            var page = Find(id);
            page.AllowAnonymous = true;
            Update(page);
        }

        public virtual void SetToDefault(int webID, int pageID)
        {
            throw new NotImplementedException();
            //var web = Context.Webs.Find(webID);
            //var page = Context.WebPages.Find(pageID);
            //web.DefaultUrl = page.Path;
            //if (IsOwnContext)
            //    Context.SaveChanges();
        }

        //public virtual IEnumerable<WebPageRole> GetRoles(int[] ids)
        //{
        //    return Context.WebPageRoles.Where(w => ids.Contains(w.PageID)).ToList();
        //}

        public int Publish(int id, string remarks = "")
        {
            if (id == 0)
                throw new ArgumentNullException("id");

            var page = Find(id);
            if (page.ID <= 0)
                throw new Exception("The page is not found in the database.You need to create this page first.");

            if (page.EnableVersioning)
            {
                var ver = page.Version + 1;
                page.Version++;
                context.Set<WebPageVersion>().Add(new WebPageVersion()
                {
                    Version = ver,
                    Published = DateTime.Now,
                    PageID = page.ID,
                    Page = page,
                    Content = page.ToXml(),
                    Remarks = remarks
                });
                Context.SaveChanges();
                Context.Database.ExecuteSqlCommand("DELETE FROM dna_Widgets WHERE TrackState=2 AND PageID={0}", id);
                var shadows = page.Widgets.Where(w => w.RefID > 0);
                foreach (var s in shadows)
                    Context.Database.ExecuteSqlCommand("DELETE FROM dna_Widgets WHERE ID={0}", s.RefID);
                Context.Database.ExecuteSqlCommand("UPDATE dna_Widgets SET TrackState=0, RefID=null WHERE PageID={0}", id);
                return ver;
            }

            return 1;
        }

        public WebPage Rollback(int pageID, int version)
        {
            var page = Find(pageID);

            if (!page.EnableVersioning)
                throw new Exception("The web page is not enable version controll.Roll back fail.");

            if (page.Version == version)
                throw new Exception("Can not roll back to current version");

            var pageVer = context.Set<WebPageVersion>().FirstOrDefault(v => v.PageID == pageID && v.Version == version);

            if (pageVer == null)
                throw new Exception("The web page version not found.");

            Context.Database.ExecuteSqlCommand("DELETE FROM dna_Widgets WHERE PageID={0}");
            Context.Database.ExecuteSqlCommand("DELETE FROM dna_WebPageVersions WHERE PageID={0} AND Version<{1}", pageID, version);

            PageElement pageData = null;

            if (!string.IsNullOrEmpty(pageVer.Content))
            {
                pageData = (PageElement)XmlSerializerUtility.DeserializeFromXmlText<PageElement>(pageVer.Content);
                page.Populate(pageData);
                var widgetDatas = pageData.Widgets;
                foreach (var w in widgetDatas)
                {
                    var descroptor = Context.WidgetDescriptors.FirstOrDefault(d => d.InstalledPath.Equals(w.WidgetID, StringComparison.OrdinalIgnoreCase));
                    if (descroptor != null)
                    {
                        var widget = new WidgetInstance();
                        widget.Popuple(w);
                        widget.PageID = page.ID;
                        widget.WebPage = page;
                        widget.DescriptorID = descroptor.ID;
                        widget.WidgetDescriptor = descroptor;
                        Context.Widgets.Add(widget);
                    }
                }
            }

            page.LastModified = DateTime.Now;
            page.Version = pageVer.Version;
            Context.SaveChanges();

            return page;
        }
    }
}
