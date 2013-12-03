//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Provides the methods to manage the moved urls 
    /// </summary>
    public class UrlSaver
    {
        private List<MovedUrl> movedUrls = new List<MovedUrl>();

        /// <summary>
        /// Initializes a new instance of the UrlSaver class with data context.
        /// </summary>
        /// <param name="context">The data context object.</param>
        public UrlSaver(IDataContext context) { this.DataContext = context; }

        private IDataContext DataContext { get; set; }

        /// <summary>
        /// Rename url
        /// </summary>
        /// <param name="orgUrl">The orginial url</param>
        /// <param name="newUrl">The new url</param>
        public void Rename(string orgUrl, string newUrl)
        {
            if (string.IsNullOrEmpty(orgUrl))
                throw new ArgumentNullException("orgUrl");

            if (string.IsNullOrEmpty(newUrl))
                throw new ArgumentNullException("newUrl");

            if (orgUrl.Equals(newUrl, StringComparison.OrdinalIgnoreCase))
                return;

            var existsUrl = GetUrl(orgUrl);

            if (existsUrl != null)
            {
                existsUrl.Url = newUrl;
            }
            else
            {
                var web = App.Get().CurrentWeb != null ? App.Get().CurrentWeb : App.Get().Webs["home"];
                DataContext.Add(new MovedUrl()
                {
                    Url = newUrl.ToLower(),
                    OriginalUrl = orgUrl.ToLower(),
                    DateCreated = DateTime.Now,
                    WebID = web.Id
                });
            }

            var reNews = DataContext.Where<MovedUrl>(m => m.Url.Equals(orgUrl, StringComparison.OrdinalIgnoreCase));
            foreach (var r in reNews)
                r.Url = newUrl;

            DataContext.SaveChanges();
        }

        /// <summary>
        /// Get renamed url object.
        /// </summary>
        /// <param name="url">The request url.</param>
        /// <returns></returns>
        public MovedUrl GetUrl(string url)
        {
            var movedUrl = movedUrls.FirstOrDefault( m => m.OriginalUrl.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (movedUrl == null)
            {
                movedUrl = DataContext.Find<MovedUrl>(m => m.OriginalUrl.Equals(url, StringComparison.OrdinalIgnoreCase));
                if (movedUrl != null)
                    movedUrls.Add(movedUrl);
            }
            return movedUrl;
        }

        /// <summary>
        /// Redirect the renamed url to new url.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>If the current url was renamed and redirected that returns true.</returns>
        public bool Redirect(HttpContextBase context)
        {
            var movedUrl = GetUrl(context.Request.Url.ToString().ToLower());
            if (movedUrl != null)
            {
                context.Response.StatusCode = 301;
                context.Response.StatusDescription = "Moved Permanently";
                context.Response.AppendHeader("Location", movedUrl.Url);
                context.Response.AppendHeader("Cache-Control", "no-cache");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Identity whether the specified request url was moved.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>Returns true when request url is moved.</returns>
        public bool IsMoved(HttpRequestBase request)
        {
            return IsMoved(request.Url.ToString().ToLower());
        }

        /// <summary>
        /// Identity whether the specified request url was moved.
        /// </summary>
        /// <param name="url">The request url</param>
        /// <returns>Returns true when request url is moved.</returns>
        public bool IsMoved(string url)
        {
            return this.GetUrl(url) != null;
        }
    }
}
