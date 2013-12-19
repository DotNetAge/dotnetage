//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Describe the web resource info.
    /// </summary>
    public class WebResourceInfo //:INavigable
    {
        /// <summary>
        /// Gets/Sets the resource uri.
        /// </summary>
        public Uri Url { get; set; }

        public WebResourceInfo(string url)
        {
            Url = new Uri(url);
        }

        public WebResourceInfo(Uri url) { Url = url; }

        /// <summary>
        /// Gets the resource owner
        /// </summary>
        public string Owner
        {
            get
            {
                if (Url.IsAbsoluteUri)
                {
                    if (Url.Host.ToLower() == "home")
                        return "";
                    else
                        return Url.Host;
                }
                return "";
            }
        }

        /// <summary>
        /// Gets the resource content type.
        /// </summary>
        public string ContentType
        {
            get
            {
                return GetContentTypeByExtension(Extension);
            }
        }

        /// <summary>
        /// Gets/Sets the resource size.
        /// </summary>
        public long FileSize
        {
            get
            {
                return filesize;
            }
            set
            {
                filesize = value;
            }
        }
        private long filesize = 0;

        public decimal GetSize(out string unit)
        {
            var size = Convert.ToDecimal(this.FileSize);
            unit = "bytes";

            if (size > 1024)
            {
                size = size / (decimal)1024;
                unit = "kb";
            }

            if (size > 1024)
            {
                size = size / (decimal)1024;
                unit = "mb";
            }

            if (size > 1024)
            {
                size = size / (decimal)1024;
                unit = "gb";
            }

            return size;
        }

        /// <summary>
        /// Gets wheather this resource is point to a file.
        /// </summary>
        public bool IsFile
        {
            get { return !string.IsNullOrEmpty(Extension); }
        }

        /// <summary>
        /// Gets the resource file name or path name.
        /// </summary>
        public string Name { get { return System.IO.Path.GetFileName(Url.ToString()); } }

        /// <summary>
        /// Gets the resource file extension.
        /// </summary>
        public string Extension
        {
            get
            {
                return System.IO.Path.GetExtension(Url.LocalPath.ToString()).ToLower();
            }
        }

        //string INavigable.Title
        //{
        //    get { return this.Name;}
        //}

        //string INavigable.Description
        //{
        //    get { return this.ContentType; }
        //}

        //string INavigable.NavigateUrl
        //{
        //    get { return this.Url.ToString(); }
        //}

        //string INavigable.ImageUrl
        //{
        //    get { return ""; }
        //}

        //string INavigable.Target
        //{
        //    get { return "_self"; }
        //}

        //object INavigable.Value
        //{
        //    get { return this.Url; }
        //}

        public static string GetContentTypeByExtension(string extension)
        {
            string mime = "application/octetstream";
            var mimes = new Dictionary<string, string>();
            #region mimetypes
            mimes.Add(".jpg", "image/jpeg");
            mimes.Add(".jpeg", "image/jpeg");
            mimes.Add(".jpe", "image/jpeg");
            mimes.Add(".png", "image/png");
            mimes.Add(".pnz", "image/png");
            mimes.Add(".tiff", "image/tiff");
            mimes.Add(".tif", "image/tiff");
            mimes.Add(".ico", "image/x-icon");
            mimes.Add(".bmp", "image/bmp");
            mimes.Add(".dib", "image/bmp");
            mimes.Add(".gif", "image/gif");
            mimes.Add(".atom", "application/atom+xml");
            mimes.Add(".jar", "application/java-archive");
            mimes.Add(".one", "application/onenote");
            mimes.Add(".onea", "application/onenote");
            mimes.Add(".onepkg", "application/onenote");
            mimes.Add(".onetmp", "application/onenote");
            mimes.Add(".oneea", "application/onenote");
            mimes.Add(".oneoc", "application/onenote");
            mimes.Add(".oneoc2", "application/onenote");
            mimes.Add(".pdf", "application/pdf");
            mimes.Add(".rtf", "application/rtf");
            mimes.Add(".xla", "application/vnd.ms-excel");
            mimes.Add(".xlc", "application/vnd.ms-excel");
            mimes.Add(".xlm", "application/vnd.ms-excel");
            mimes.Add(".xls", "application/vnd.ms-excel");
            mimes.Add(".xlt", "application/vnd.ms-excel");
            mimes.Add(".xlw", "application/vnd.ms-excel");
            mimes.Add(".pot", "application/vnd.ms-powerpoint");
            mimes.Add(".ppt", "application/vnd.ms-powerpoint");
            mimes.Add(".pps", "application/vnd.ms-powerpoint");
            //mimes.Add(".pot","application/vnd.ms-powerpoint");
            mimes.Add(".doc", "application/msword");
            mimes.Add(".dot", "application/msword");
            mimes.Add(".xaml", "application/xaml+xml");
            mimes.Add(".gtar", "application/x-gtar");
            mimes.Add(".gz", "application/x-gzip");
            mimes.Add(".class", "application/x-java-applet");
            //mimes.Add(".js","application/x-javascript");
            mimes.Add(".zip", "application/x-zip-compressed");
            mimes.Add(".mp3", "audio/mpeg");
            mimes.Add(".aifc", "audio/aiff");
            mimes.Add(".aiff", "audio/aiff");
            mimes.Add("au", "audio/basic");
            mimes.Add(".snd", "audio/basic");
            mimes.Add(".mid", "audio/mid");
            mimes.Add(".midi", "audio/mid");
            mimes.Add(".rmi", "audio/mid");
            mimes.Add(".wav", "audio/wav");
            mimes.Add(".aif", "audio/x-aiff");
            mimes.Add(".m3u", "audio/x-mpegurl");
            mimes.Add(".wax", "audio/x-ms-wax");
            mimes.Add(".wma", "audio/x-ms-wma");
            mimes.Add(".ra", "audio/x-pn-realaudio");
            mimes.Add(".ram", "audio/x-pn-realaudio");
            mimes.Add(".rpm", "audio/x-pn-realaudio-plugin");
            mimes.Add(".htm", "text/html");
            mimes.Add(".html", "text/html");
            mimes.Add(".hxt", "text/html");
            mimes.Add(".asm", "text/plain");
            mimes.Add(".bas", "text/plain");
            mimes.Add(".c", "text/plain");
            mimes.Add(".cnf", "text/plain");
            mimes.Add(".cpp", "text/plain");
            mimes.Add(".h", "text/plain");
            mimes.Add(".map", "text/plain");
            mimes.Add(".txt", "text/plain");
            mimes.Add(".vcs", "text/plain");
            mimes.Add(".xdr", "text/plain");
            mimes.Add(".rtx", "text/richtext");
            mimes.Add(".css", "text/css");
            mimes.Add(".sgml", "text/sgml");
            mimes.Add(".vbs", "text/vbscript");
            mimes.Add(".js", "text/javascript");
            mimes.Add(".htc", "text/x-component");
            mimes.Add(".xml", "text/xml");
            mimes.Add(".dtd", "text/xml");
            mimes.Add(".disco", "text/xml");
            mimes.Add(".dll.config", "text/xml");
            mimes.Add(".exec.config", "text/xml");
            mimes.Add(".mno", "text/xml");
            mimes.Add(".vml", "text/xml");
            mimes.Add(".wsdl", "text/xml");
            mimes.Add(".xsd", "text/xml");
            mimes.Add(".xsl", "text/xml");
            mimes.Add(".xslt", "text/xml");
            mimes.Add(".m1v", "video/mpeg");
            mimes.Add(".mp2", "video/mpeg");
            mimes.Add(".mpa", "video/mpeg");
            mimes.Add(".mpe", "video/mpeg");
            mimes.Add(".mpeg", "video/mpeg");
            mimes.Add(".mpg", "video/mpeg");
            mimes.Add(".mpv2", "video/mpeg");
            mimes.Add(".mov", "video/quicktime");
            mimes.Add(".qt", "video/quicktime");
            mimes.Add(".mp4", "video/mp4");
            mimes.Add(".webm", "video/webm");
            mimes.Add(".ogg", "video/ogg");
            mimes.Add(".3gp", "video/3gpp");
            mimes.Add(".ivf", "video/x-ivf");
            mimes.Add(".flv", "video/x-flv");
            mimes.Add(".lsf", "video/x-la-asf");
            mimes.Add(".lsx", "video/x-asf");
            mimes.Add(".asf", "video/x-ms-asf");
            mimes.Add(".asr", "video/x-ms-asf");
            mimes.Add(".asx", "video/x-ms-asf");
            mimes.Add(".nsc", "video/x-ms-asf");
            mimes.Add(".avi", "video/x-msvideo");
            mimes.Add(".wm", "video/x-ms-wm");
            mimes.Add(".wmp", "video/x-ms-wmp");
            mimes.Add(".wmv", "video/x-ms-wmv");
            mimes.Add(".wmx", "video/x-ms-wmx");
            mimes.Add(".movie", "video/x-sgi-movie");
            #endregion

            if (mimes.ContainsKey(extension))
                return mimes[extension];
            else
                return mime;
        }
    }
}
