//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.ServiceModel
{
    public static class PermissionLoader
    {
        //        private static IDataContext context;
        //      private static string _targetPath = "";

        private static IDataContext context
        {
            get
            {
                return App.GetService<IDataContext>();
            }
        }
        public static void Load(string targetPath = "")
        {
            string _targetPath = "";
            if (string.IsNullOrEmpty(targetPath))
                _targetPath = HttpContext.Current.Server.MapPath("~/bin");
            else
                _targetPath = targetPath;

            string[] files = Directory.GetFiles(_targetPath, "*.dll");
            foreach (string file in files)
            {
                try
                {
                    //When using LoadFile will cause could not get CustomAttributes!
                    Assembly assembly = Assembly.LoadFrom(file);
                    AssemblyName asmname = assembly.GetName();
                    Type[] types = assembly.GetTypes();
                    var controllers = from c in types
                                      where c.BaseType == typeof(Controller)
                                      select c;

                    Dictionary<string, string> added = new Dictionary<string, string>();

                    foreach (Type controller in controllers)
                    {
                        var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        var actions = from MethodInfo method in methods
                                      where (method.GetCustomAttributes(typeof(SecurityActionAttribute), true).Length > 0)
                                      select method;

                        foreach (MethodInfo action in actions)
                        {
                            SecurityActionAttribute attr = (SecurityActionAttribute)Attribute.GetCustomAttribute(action, typeof(SecurityActionAttribute));

                            var instance = context.Permissions.Filter(p => (p.Action.Equals(action.Name, StringComparison.OrdinalIgnoreCase)) &&
                                           (p.Assembly.Equals(asmname.Name, StringComparison.OrdinalIgnoreCase)) &&
                                           (p.Controller.Equals(controller.FullName, StringComparison.OrdinalIgnoreCase)) &&
                                           (p.Title.Equals(attr.Title, StringComparison.OrdinalIgnoreCase)));

                            if (instance.Count() > 0)
                                continue;

                            string _key = asmname.Name + "_" + controller.FullName + "_" + action.Name;
                            if (added.ContainsKey(_key))
                            {
                                if (added[_key] == attr.Title)
                                    continue;
                            }
                            else
                                added.Add(_key, attr.Title);

                            Permission permission = new Permission()
                            {
                                Action = action.Name,
                                Assembly = asmname.Name,
                                Controller = controller.FullName,
                                Title = attr.Title,
                                Description = attr.Description
                            };

                            PermissionSet pset = null;
                            if (!string.IsNullOrEmpty(attr.PermssionSet))
                                pset = context.Find<PermissionSet>(p => p.Name.Equals(attr.PermssionSet, StringComparison.OrdinalIgnoreCase));

                            //var _updateCount = 0;

                            if (pset == null)
                            {
                                pset = new PermissionSet();
                                pset.Name = attr.PermssionSet;
                                pset.ResbaseName = attr.ResBaseName;
                                pset.TitleResName = attr.PermssionSetResName;
                                pset = context.Add(pset);
                                //_updateCount=context.SaveChanges();
                            }

                            permission.PermissionSet = pset;
                            context.Permissions.Create(permission);
                            context.SaveChanges();
                        }
                    }
                }
                catch { continue; }
            }

            RemoveUsingPermissions();
        }

        private static void RemoveUsingPermissions()
        {
            var perms = context.Permissions.All().ToList();
            var isChanged = false;
            foreach (var perm in perms)
            {
                var typeStr = perm.Controller + "," + perm.Assembly;
                //var asm = Assembly.LoadWithPartialName(perm.Assembly);
                var type = Type.GetType(typeStr);
                if (type == null)
                {
                    context.Permissions.Delete(perm);
                    isChanged = true;
                    continue;
                }

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var actions = methods.Where(m => m.Name.Equals(perm.Action));

                if (actions.Count() == 0)
                {
                    context.Permissions.Delete(perm);
                    isChanged = true;
                    continue;
                }

                var hasAttr = false;
                foreach (var action in actions)
                {
                    SecurityActionAttribute attr = Attribute.GetCustomAttribute(action, typeof(SecurityActionAttribute)) as SecurityActionAttribute;
                    if (attr != null)
                    {
                        hasAttr = true;
                        break;
                    }
                }

                if (hasAttr) continue;

                context.Permissions.Delete(perm);
                isChanged = true;
            }

            var permSets = context.All<PermissionSet>();

            foreach (var permset in permSets)
            {
                if ((permset.Permissions != null) && (permset.Permissions.Count == 0))
                {
                    context.Delete(permset);
                    isChanged = true;
                }
            }

            if (isChanged)
                context.SaveChanges();
        }
    }
}
