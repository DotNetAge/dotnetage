//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity
{
    public class WidgetDescriptorRepository : EntityRepository<WidgetDescriptor>, IWidgetDescriptorRepository
    {
        public WidgetDescriptorRepository() : base() { }

        public WidgetDescriptorRepository(CoreDbContext dbContext) : base(dbContext) { }

        public virtual void Delete(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            this.Delete(Find(id));
        }

        public virtual void Delete(string installedPath)
        {
            if (string.IsNullOrEmpty(installedPath)) throw new ArgumentNullException("installedPath");
            this.Delete(Find(installedPath));
        }

        public virtual WidgetDescriptor Find(string controllerName, string action)
        {
            return Find(w => (w.Controller == controllerName && w.Action == action));
        }

        public virtual WidgetDescriptor Find(string installedPath)
        {
            if (string.IsNullOrEmpty(installedPath)) throw new ArgumentNullException("installedPath");
            return Find(w => (w.InstalledPath.Equals(installedPath, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual IEnumerable<WidgetDescriptor> WithInPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            Context.Configuration.ProxyCreationEnabled = false;
            return Filter(w => w.InstalledPath.Contains(path));
        }

        public virtual int InusingWidgetsCount(string installedPath)
        {
            if (string.IsNullOrEmpty(installedPath)) throw new ArgumentNullException("installedPath");
            var descriptor = Find(installedPath);
            if (descriptor == null)
                return 0;
            return ((CoreDbContext)Context).Widgets.Count(w => w.DescriptorID == descriptor.ID);
        }

 
        private string ResolveInstalledPathUrl(string vPath, string installedPath)
        {
            if (!string.IsNullOrEmpty(vPath))
            {
                if (!vPath.StartsWith("~") && !vPath.StartsWith("/"))
                    return "~/content/widgets/" + installedPath.Replace("\\", "/") + "/" + vPath;
                else
                    return vPath;
            }
            return "";
        }

        public void AddRoles(int id, string[] roles)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException("id");

            if (roles == null || roles.Length == 0)
                throw new ArgumentNullException("roles");

            var descriptor = Context.WidgetDescriptors.Find(id);

            if (descriptor == null)
                throw new Exception("WidgetDescriptor not found.");

            var rs = context.Roles.Where(r => roles.Contains(r.Name)).ToList();
            descriptor.Roles.Clear();

            foreach (var r in rs)
                descriptor.Roles.Add(r);

            if (IsOwnContext)
                Context.SaveChanges();
        }

        public override WidgetDescriptor Create(WidgetDescriptor TObject)
        {
            var descriptor = base.Create(TObject);
            
            var roles=new string[] { "administrators", "guests" };
            var rs = context.Roles.Where(r => roles.Contains(r.Name)).ToList();
            descriptor.Roles = new List<Role>(rs) {};

            if (IsOwnContext)
                Context.SaveChanges();

            return descriptor;
        }

        public string[] GetRoles(int id)
        {
            if (id == 0)
                throw new ArgumentException("id");
            var description = Find(id);
            if (description == null)
                throw new Exception("Widget descriptior not found");

            return description.Roles.Select(r => r.Name).ToArray();
        }
    }
}
