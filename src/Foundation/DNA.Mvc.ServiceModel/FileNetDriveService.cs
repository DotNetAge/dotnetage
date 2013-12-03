//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.Events;
using DNA.Web.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represents a netdrive service that implement INetDriveService.
    /// </summary>
    public class FileNetDriveService : INetDriveService
    {
        #region private methods and properties

        private Route Route
        {
            get
            {
                return (Route)RouteTable.Routes["dna_webfiles_get"];
            }
        }

        private void SetElementAttributes(XElement element, IDictionary<string, object> attributes)
        {
            foreach (var key in attributes.Keys)
            {
                if (element.Attribute(key) == null)
                    element.Add(new XAttribute(key, attributes[key]));
                else
                    element.Attribute(key).SetValue(attributes[key]);
            }
        }

        private IDictionary<string, object> GetAttributesFromNode(XmlAttributeCollection attributes)
        {
            if (attributes != null)
            {
                var attrs = new Dictionary<string, object>();
                for (int i = 0; i < attributes.Count; i++)
                {
                    var attr = attributes[i];
                    if (attr.Name.Equals("name"))
                        continue;
                    attrs.Add(attr.Name, attr.Value);
                }
                return attrs;
            }
            return null;
        }

        private long RecursiveGetSize(string path)
        {
            long size = 0;
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
                size += files[i].Length;

            var subdirs = dir.GetDirectories();
            foreach (var sub in subdirs)
                size += RecursiveGetSize(sub.FullName);
            return size;
        }

        #endregion

        /// <summary>
        /// Returns the physical file path that corresponds to the specified the resoure url on the Web server.
        /// </summary>
        /// <param name="url">The resoure url.</param>
        /// <returns> The physical file path that corresponds to path.</returns>
        public string MapPath(Uri url)
        {
            var routeData = Route.GetRouteData(DNA.Utility.UrlUtility.CreateRequestContext(url).HttpContext);
            string website = routeData.Values["website"] as string;
            string path = routeData.Values["path"] as string;
            string webPath = website.Equals("home", StringComparison.OrdinalIgnoreCase) ? "~/app_data/files/public" : "~/app_data/files/personal/" + website;
            return HttpContext.Current.Server.MapPath(webPath + "/" + path);
        }

        /// <summary>
        /// Deletes the specified file. 
        /// </summary>
        /// <param name="url">Specified the resource uri to delete.</param>
        public void Delete(Uri url)
        {
            string fn = MapPath(url);

            if (!string.IsNullOrEmpty(Path.GetExtension(fn)))
            {
                if (File.Exists(fn))
                {
                    lock (fn)
                    {
                        File.Delete(fn);
                        this.Trigger("FileDeleted", new WebFileEventArgs() { Uri = url });
                        //EventDispatcher.RaiseFileDeleted(url);
                    }
                }
            }
            else
            {
                if (Directory.Exists(fn))
                {
                    Directory.Delete(fn, true);
                    this.Trigger("PathDeleted", new WebFileEventArgs() { Uri = url });
                    //EventDispatcher.RaisePathDeleted(url);
                }
            }
        }

        /// <summary>
        /// Create the parth for specified uri.
        /// </summary>
        /// <param name="url">The directory path to create.</param>
        public void CreatePath(Uri url)
        {
            string fn = MapPath(url);
            DirectoryInfo dir = Directory.CreateDirectory(fn);
            //EventDispatcher.RaisePathCreated(url);
            this.Trigger("PathCreated", new WebFileEventArgs() { Uri = url });
        }

        /// <summary>
        ///  Save the data to destaintion path.
        /// </summary>
        /// <param name="data">The raw data of file.</param>
        /// <param name="fileName">Specified the file name.</param>
        /// <param name="destURI">the specified destaintion path where to save.</param>
        public Uri SaveFile(byte[] data, string fileName, Uri destURI)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (destURI == null)
                throw new ArgumentNullException("destURI");

            string dest = MapPath(destURI);
            string _fileName = fileName;
            if (fileName.IndexOf("/") > 0)
                _fileName = _fileName.Replace("/", "_");
            var _target = "";

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            if (dest.EndsWith("\\"))
                _target = dest + _fileName;
            else
                _target = dest + "\\" + fileName;

            File.WriteAllBytes(_target, data);

            var _newURI = "";
            if (destURI.ToString().EndsWith("/"))
                _newURI = destURI.ToString() + _fileName;
            else
                _newURI = destURI.ToString() + "/" + _fileName;

            var _newUri = new Uri(_newURI);

            //EventDispatcher.RaiseFileSaved(_newUri);
            this.Trigger("FileSaved", new WebFileEventArgs() { Uri = _newUri });
            return _newUri;
        }

        /// <summary>
        ///  Move a file or a directory and its contents to a new location.
        /// </summary>
        /// <param name="sourceUrl"> The url of the file or directory to move.</param>
        /// <param name="destUrl">The destination url to the new location for sourceURI</param>
        public void Move(Uri sourceUrl, Uri destUrl)
        {
            string src = MapPath(sourceUrl);
            string dest = MapPath(destUrl);
            bool isFile = File.Exists(src);

            //Desc url must be path url
            if (isFile)
            {
                //Move file
                var fileName = Path.GetFileName(src);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                    //EventDispatcher.RaisePathCreated(destUrl);
                    this.Trigger("PathCreated", new WebFileEventArgs() { Uri = destUrl });

                }

                var destFileName = dest + (dest.EndsWith("\\") ? "" : "\\") + fileName;

                if (File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }

                dest = destFileName;
            }
            else
            {
                //Move folder
                var dirName = (new DirectoryInfo(src)).Name;
                dest = dest + (dest.EndsWith("\\") ? "" : "\\") + dirName;
            }

            Directory.Move(src, dest);

            //  if (isFile)
            this.Trigger(isFile ? "FileRenamed" : "PathRenamed", new WebFileRenameEventArgs() { DestinationUri = destUrl, SourceUri = sourceUrl });
            // EventDispatcher.RaisePathRenamed(sourceUrl, destUrl);
            // else
            //this.Trigger("PathRenamed", new WebFileEventArgs() { Uri = destUrl });
            // EventDispatcher.RaisePathRenamed(sourceUrl, destUrl);
        }

        /// <summary>
        /// Get the file's raw data for specified filename
        /// </summary>
        /// <param name="url">The uri of the resource file</param>
        /// <returns>A byte array contains the file raw data.</returns>
        public byte[] Open(Uri url)
        {
            string file = MapPath(url);
            if (File.Exists(file))
                return File.ReadAllBytes(file);
            return null;
        }

        /// <summary>
        /// Gets the file info objects by the specified path.
        /// </summary>
        /// <param name="url">The specified path</param>
        /// <returns>A collection contains resource uri.</returns>
        public IEnumerable<Uri> GetFiles(Uri url)
        {
            string _path = MapPath(url);
            if (!Directory.Exists(_path))
                return null;

            string[] files = Directory.GetFiles(_path);
            List<Uri> fileUrls = new List<Uri>();

            foreach (string file in files)
            {
                if (file.EndsWith(".metax"))
                    continue;

                DirectoryInfo info = new DirectoryInfo(file);
                string _url = url.ToString().EndsWith("/") ? url.ToString() + info.Name : url.ToString() + "/" + info.Name;
                Uri _uri = null;
                Uri.TryCreate(_url, UriKind.RelativeOrAbsolute, out _uri);
                if (_uri != null)
                    fileUrls.Add(_uri);
            }

            return fileUrls;
        }

        /// <summary>
        ///  Gets the sub path uris by the specified path.
        /// </summary>
        /// <param name="url">The specified path.</param>
        /// <returns>A collection contains resource path uri.</returns>
        public IEnumerable<Uri> GetPaths(Uri url)
        {
            string _path = MapPath(url);
            string[] dirs = Directory.GetDirectories(_path);
            List<Uri> dirUrls = new List<Uri>();

            foreach (string dir in dirs)
            {
                DirectoryInfo info = new DirectoryInfo(dir);
                string _url = url.ToString().EndsWith("/") ? url.ToString() + info.Name : url.ToString() + "/" + info.Name;
                dirUrls.Add(new Uri(_url));
            }
            return dirUrls;
        }

        /// <summary>
        /// Identity whether the specified url path or file is exist.
        /// </summary>
        /// <param name="url">The url object</param>
        /// <returns>If the path of file exists return true.</returns>
        public bool Exists(Uri url)
        {
            var filePath = MapPath(url);
            var exists = Directory.Exists(filePath);
            if (!exists)
                exists = File.Exists(filePath);
            return exists;
        }

        /// <summary>
        /// Copy the specified url file(path) to desctination.
        /// </summary>
        /// <param name="source">The source uri</param>
        /// <param name="destination">The destination uri</param>
        public void Copy(Uri source, Uri destination)
        {
            string srcPath = MapPath(source);
            string destPath = MapPath(destination);
            if (Path.HasExtension(srcPath)) //Path
            {
                File.Copy(srcPath, destPath, true);
                this.Trigger("FileSaved", new WebFileEventArgs() { Uri = destination });
                //EventDispatcher.RaiseFileSaved(destination);
            }
            else
            {
                FileUtility.CopyDirectory(srcPath, destPath);
                this.Trigger("PathCreated", new WebFileEventArgs() { Uri = destination });

                //EventDispatcher.RaisePathCreated(destination);
            }
        }

        /// <summary>
        /// Rename the specified uri file(path) to new name.
        /// </summary>
        /// <param name="source">The source file/path uri.</param>
        /// <param name="name">The file/path name.</param>
        /// <returns>The new uri path will return.</returns>
        public Uri Rename(Uri source, string name)
        {
            var path = MapPath(source);
            //var dirInfo = new DirectoryInfo(path);
            if (Path.HasExtension(path)) //Rename file
            {
                var fileName = Path.GetFileName(path);
                var destPath = path.Replace(fileName, name);
                File.Copy(path, destPath);
                File.Delete(path);
                var newFileUri = new Uri(source.ToString().Replace(fileName, name));
                //EventDispatcher.RaiseFileRenamed(source, newFileUri);
                this.Trigger("FileRenamed", new WebFileRenameEventArgs() { SourceUri =source, DestinationUri =newFileUri  });
                return newFileUri;
            }
            else
            {
                var dir = new DirectoryInfo(path);
                var dirName = dir.Name;
                var destName = dir.Parent.FullName + (!dir.Parent.FullName.EndsWith("/") ? "/" : "") + name;
                dir.MoveTo(destName);
                var newSegments = new string[source.Segments.Length];
                source.Segments.CopyTo(newSegments, 0);
                newSegments[source.Segments.Length - 1] = name;
                var newUrl = new Uri(source.Scheme + "://" + source.Authority + string.Join("", newSegments));
                //EventDispatcher.RaisePathRenamed(source, newUrl);
                this.Trigger("PathRenamed", new WebFileRenameEventArgs() { SourceUri = source, DestinationUri = newUrl  });
                return newUrl;
            }
        }

        /// <summary>
        /// Gets the additional attributes from specified url
        /// </summary>
        /// <param name="url">The file/path uri</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAttributes(Uri url)
        {
            var file = MapPath(url);
            var isFile = Path.HasExtension(file) ? true : false;
            var path = isFile ? Path.GetDirectoryName(file) : file;
            var dirInfo = new DirectoryInfo(path);

            var infoFile = path + "\\" + dirInfo.Name + ".metax";


            if (File.Exists(infoFile))
            {
                var xdoc = new XmlDocument();
                xdoc.Load(infoFile);
                if (isFile)
                {
                    var name = Path.GetFileName(file);
                    var node = xdoc.SelectSingleNode("/files/file[@name='" + name + "']");
                    if (node != null)
                        return GetAttributesFromNode(node.Attributes);
                }
                else
                {
                    return GetAttributesFromNode(xdoc.DocumentElement.Attributes);
                }

            }
            return null;
        }

        /// <summary>
        /// Set the additional attributes by specified url and name value dictionary.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="attributes"></param>
        public void SetAttributes(Uri url, IDictionary<string, object> attributes)
        {
            var file = MapPath(url);
            var isFile = Path.HasExtension(file) ? true : false;
            var path = isFile ? Path.GetDirectoryName(file) : file;
            var dirInfo = new DirectoryInfo(path);
            var infoFile = path + "\\" + dirInfo.Name + ".metax";

            if (File.Exists(infoFile))
            {
                var xdoc = XDocument.Load(infoFile);
                var root = xdoc.Element("files");

                if (isFile)
                {
                    var name = Path.GetFileName(file);
                    var element = root.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals(name));
                    if (element == null)
                    {
                        element = new XElement("file");
                        element.Add(new XAttribute("name", name));
                        root.Add(element);
                    }
                    SetElementAttributes(element, attributes);
                }
                else
                {
                    SetElementAttributes(root, attributes);
                }
                xdoc.Save(infoFile);
            }
            else
            {
                var xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
                var root = new XElement("files");
                root.Add(new XAttribute("name", dirInfo.Name));
                xdoc.Add(root);
                if (isFile)
                {
                    var fElement = new XElement("file");
                    fElement.Add(new XAttribute("name", Path.GetFileName(file)));
                    SetElementAttributes(fElement, attributes);
                    root.Add(fElement);
                }
                else
                {
                    SetElementAttributes(root, attributes);
                }
                xdoc.Save(infoFile);
            }
        }

        /// <summary>
        /// Gets the specified file size
        /// </summary>
        /// <param name="url">The file uri.</param>
        /// <returns>A long value contains total bytes of the file.</returns>
        public long GetFileSize(Uri url)
        {
            if (url != null)
            {
                var path = MapPath(url);
                if (System.IO.File.Exists(path))
                {
                    var f = new System.IO.FileInfo(path);
                    return f.Length;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the specified directory size.
        /// </summary>
        /// <param name="url">The dir uri</param>
        /// <returns>A long value contains total bytes of the specified directory</returns>
        public long GetDirectorySize(Uri url)
        {

            if (url != null)
            {
                var path = MapPath(url);
                if (Directory.Exists(path))
                    return RecursiveGetSize(path);
            }
            return 0;
        }

        /// <summary>
        ///  Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="url">The file to open for reading.</param>
        /// <returns>A string containing all lines of the file.</returns>
        public string ReadText(Uri url)
        {
            string file = MapPath(url);
            if (File.Exists(file))
                return File.ReadAllText(file);
            return null;
        }

        /// <summary>
        ///  Reads the lines of a file.
        /// </summary>
        /// <param name="url"> The file to read.</param>
        /// <returns> The lines of the file.</returns>
        public string[] ReadLines(Uri url)
        {
            string file = MapPath(url);
            if (File.Exists(file))
                return File.ReadAllLines(file);
            return null;
        }

        /// <summary>
        ///     Creates a new file, writes the specified string to the file, and then closes
        ///     the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="url"> The file to write to.</param>
        /// <param name="contents">The content to write to the file</param>
        public void WriteText(Uri url, string contents)
        {
            string file = MapPath(url);
            File.WriteAllText(file, contents, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new file, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        /// <param name="url">The file to write to.</param>
        /// <param name="lines">The lines to write to the file.</param>
        public void WriteLines(Uri url, string[] lines)
        {
            string file = MapPath(url);
            File.WriteAllLines(file, lines, System.Text.Encoding.UTF8);
        }
    }
}
