//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    ///  Represents an attribute that is used to cache the web resource files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WebFileCacheAttribute : ActionFilterAttribute
    {
        private int duration = 300;

        /// <summary>
        /// Gets/Sets the cache duration in seconds.
        /// </summary>
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var response = filterContext.RequestContext.HttpContext.Response;
            
            //Format the request url without querystring
            var urlString = request.Url.ToString();
            if (!string.IsNullOrEmpty(request.Url.Query))
                urlString = urlString.Replace(request.Url.Query, "");
            var url = new Uri(urlString);
            var webres = new WebResourceInfo(url);
            
            if (webres.IsFile)
            {
                var noneMatch = request.Headers["If-None-Match"];
                var isNotMatch =true;
                if (!string.IsNullOrEmpty(noneMatch))
                {
                    var service = DependencyResolver.Current.GetService<INetDriveService>();
                    var file = service.MapPath(url);
                    var etagHashed = GetEtag(file);
                    isNotMatch =  !noneMatch.Equals(etagHashed);
                }

                var isExpried=!(request.Headers["If-Modified-Since"] != null && TimeSpan.FromTicks(DateTime.Now.Ticks - DateTime.Parse(request.Headers["If-Modified-Since"]).Ticks).Seconds < Duration);

                if (!isNotMatch && !isExpried)
                {
                    try
                    {
                        response.Write(DateTime.Now);
                        response.StatusCode = 304;
                        response.Headers.Add("Content-Encoding", "gzip");
                        response.StatusDescription = "Not Modified";
                    }
                    catch //(System.PlatformNotSupportedException e)
                    {
                        base.OnActionExecuting(filterContext);
                    }
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            else
                base.OnActionExecuting(filterContext);

        }
        
        private string GetEtag(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var etag = fileInfo.FullName.ToLower() + fileInfo.Length.ToString() +fileInfo.CreationTime.ToString()+ fileInfo.LastWriteTime.ToString();
            var md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            var hashed= md5.ComputeHash(Encoding.ASCII.GetBytes(etag));
            var sign = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                sign.Append(hashed[i].ToString("X2"));
            }
            return sign.ToString();
        }

        private void SetFileCaching(HttpResponseBase response, string fileName)
        {
            response.Headers.Set("ETag", GetEtag(fileName));
            var fileInfo = new FileInfo(fileName);
            response.Headers.Set("Last-Modified", fileInfo.LastWriteTime.ToString("R"));
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
            response.Cache.SetSlidingExpiration(true);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as FilePathResult;
            if (result != null)
            {
                if (!string.IsNullOrEmpty(result.FileName) && (System.IO.File.Exists(result.FileName)))
                    SetFileCaching(filterContext.HttpContext.Response, result.FileName);
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
