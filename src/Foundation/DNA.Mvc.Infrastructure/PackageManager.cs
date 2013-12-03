//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DNA.Web
{
    public class PackageManager<T, TFactory, TElement> : IEnumerable<T>
        where T : PackageBase<TElement>
        where TFactory : IPackageFactory<T, TElement>
        where TElement : class
    {
        public string InstalledPath { get; protected set; }

        public virtual void Init(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException();

            InstalledPath = path;
            var dirs = Directory.GetDirectories(path);
            var packages = new PackageCollection<T, TElement>();
            var factory = Activator.CreateInstance<TFactory>();
            foreach (var dir in dirs)
            {
                try
                {
                    packages.Add(factory.Create(dir));
                }
                catch { continue; }
            }

            Packages = packages;
        }

        public virtual T this[string name] { get { return this.Packages[name]; } }

        public virtual PackageCollection<T, TElement> Packages { get; protected set; }

        public T this[int index]
        {
            get
            {
                return Packages[index];
            }
        }

        /// <summary>
        /// Download the page from DotNetAge Gallery
        /// </summary>
        /// <param name="url"></param>
        public virtual void Download(string url) 
        {
            var client = new WebClient();
            var remoteUrl=new Uri(url);
            var fileName=Path.GetFileName(url);
            var fullName = this.InstalledPath + "\\" + fileName;
            client.DownloadFileAsync(remoteUrl, fullName,fullName);
            client.DownloadFileCompleted += client_DownloadFileCompleted;
        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var fullName = (string)e.UserState;
            var name = Path.GetFileNameWithoutExtension(fullName);
            OnFileDownloadCompleted(name, fullName);
        }

        public virtual void OnFileDownloadCompleted(string name, string fileName) 
        {
        }

        /// <summary>
        /// Delete the package and it's files by specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Delete(string name)
        {
            var pkg = Packages[name];
            if (pkg == null)
                return false;

            if (Directory.Exists(pkg.InstalledPath))
            {
                try
                {
                    Directory.Delete(pkg.InstalledPath);
                    return true;
                }
                catch { return false; }
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Packages.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}
