//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web
{
    /// <summary>
    /// Cache the file with on browser side.
    /// </summary>
    /// <remarks>
    /// This attribute only avaliable when your file is directly returns from IIS.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FileCacheAttribute : ActionFilterAttribute
    {
        private int duration = 300;

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var response = filterContext.RequestContext.HttpContext.Response;
            if (request.Headers["If-Modified-Since"] != null && TimeSpan.FromTicks(DateTime.Now.Ticks - DateTime.Parse(request.Headers["If-Modified-Since"]).Ticks).Seconds < Duration)
            {
                try
                {
                    response.Write(DateTime.Now);
                    response.StatusCode = 304;
                    response.Headers.Add("Content-Encoding", "gzip");
                    response.StatusDescription = "Not Modified";
                }
                catch // (System.PlatformNotSupportedException e)
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }

        private void SetFileCaching(HttpResponseBase response, string fileName)
        {
            response.AddFileDependency(fileName);
            response.Cache.SetETagFromFileDependencies();
            response.Cache.SetLastModifiedFromFileDependencies();
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