//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DNA.Web
{
    /// <summary>
    /// Represent a module descriptor class that use to get the register module info.
    /// </summary>
    public class ModuleDescriptor
    {
        private string title;
        private string copyright;
        private string compay;
        private string description;
        private bool _loaded = false;
        private string icon = "";

        /// <summary>
        /// Gets/Sets the solution register name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the module assembly qualified name.
        /// </summary>
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/Sets the solution assembly name.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets/Sets the solution assembly qualified name.
        /// </summary>
        public string AssemblyFullName { get; set; }

        /// <summary>
        /// Gets /Sets the solution route name.
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Gets/Sets the solution build version.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Indicates whether the module is disabled.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets the solution title that loads from solution assembly
        /// </summary>
        public string Title
        {
            get { if (!_loaded) LoadAssemblyInfo(); return title; }
            set { title = value; }
        }

        /// <summary>
        /// Gets the description that loads from solution assembly
        /// </summary>
        public string Description
        {
            get { if (!_loaded) LoadAssemblyInfo(); return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets the company name that loads from solution assembly
        /// </summary>
        public string Company
        {
            get { if (!_loaded) LoadAssemblyInfo(); return compay; }
            set { compay = value; }
        }

        /// <summary>
        /// Gets the solution copyright info
        /// </summary>
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        /// <summary>
        /// Gets the solution author name.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets the solution supported url.
        /// </summary>
        public string SupportLink { get; set; }

        internal void LoadAssemblyInfo()
        {
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Equals(this.AssemblyFullName, StringComparison.OrdinalIgnoreCase));
                var attrs = assembly.GetCustomAttributes(true);
                var attr_title = attrs.FirstOrDefault(a => a.GetType().Equals(typeof(AssemblyTitleAttribute)));
                var attr_desc = attrs.FirstOrDefault(a => a.GetType().Equals(typeof(AssemblyDescriptionAttribute)));
                var attr_company = attrs.FirstOrDefault(a => a.GetType().Equals(typeof(AssemblyCompanyAttribute)));
                var attr_copyright = attrs.FirstOrDefault(a => a.GetType().Equals(typeof(AssemblyCopyrightAttribute)));

                if (attr_title != null)
                    title = ((AssemblyTitleAttribute)attr_title).Title;

                if (attr_desc != null)
                    description = ((AssemblyDescriptionAttribute)attr_desc).Description;

                if (attr_company != null)
                    compay = ((AssemblyCompanyAttribute)attr_company).Company;

                if (attr_copyright != null)
                    copyright = ((AssemblyCopyrightAttribute)attr_copyright).Copyright;

                Version = assembly.GetName().Version;
            }
            catch { }
        }

        /// <summary>
        /// Gets icon in assembly resources.
        /// </summary>
        public string Icon
        {
            get
            {
                if (string.IsNullOrEmpty(icon))
                {
                    var iconName = "favicon.ico";
                    var iconData = GetResourceData(iconName);
                    if (iconData != null)
                    {
                        var prefix = "data:image/x-icon;base64,";
                        icon = prefix + Convert.ToBase64String(iconData);
                    }
                    else
                    {
                        iconName = "dna-module-icon.png";
                        iconData = GetResourceData(iconName);
                        if (iconData != null)
                        {
                            var prefix = "data:image/png;base64,";
                            icon = prefix + Convert.ToBase64String(iconData);
                        }
                    }
                }
                return icon;
            }
        }

        /// <summary>
        /// Gets stream object from specified resource name.
        /// </summary>
        /// <param name="name">The resource key name.</param>
        /// <returns></returns>
        public Stream GetResourceStream(string name)
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Equals(this.AssemblyFullName, StringComparison.OrdinalIgnoreCase));
            var resNames = asm.GetManifestResourceNames();
            var _name = this.AssemblyName + "." + name;
            if (resNames.Contains(_name))
                return asm.GetManifestResourceStream(_name);
            return null;
        }

        /// <summary>
        /// Gets resource string by specified name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <returns>A string value.</returns>
        public string GetResourceString(string name)
        {
            var stream = GetResourceStream(name);
            if (stream != null)
            {
                var str = "";
                using (var reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
                stream.Close();
                return str;
            }
            return "";
        }

        /// <summary>
        /// Gets binary value by specified name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <returns>A byte array.</returns>
        public byte[] GetResourceData(string name)
        {
            var stream = GetResourceStream(name);
            if (stream != null)
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Close();
                return bytes;
            }
            return null;
        }
    }
}
