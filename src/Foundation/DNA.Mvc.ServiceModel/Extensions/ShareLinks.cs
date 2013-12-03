//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.WebPages;
using System.Web.WebPages.Scope;

namespace DNA.Web.UI
{
    /// <summary>
    /// The helper page to create share link elements.
    /// </summary>
    public class ShareLinks : HelperPage
    {
        static ShareLinks()
        {
            _bitlyApiKey = new object();
            _bitlyLogin = new object();
            //_allSites = new Lazy<IEnumerable<LinkShareSite>>(() => from site in (LinkShareSite[]) Enum.GetValues(typeof(LinkShareSite))
            //    where site != LinkShareSite.All
            //    select site);
        }

        public ShareLinks() { }

        /// <summary>
        /// Render the share link elements for social networks.
        /// </summary>
        /// <param name="pageTitle">The link page title.</param>
        /// <param name="pageLinkBack">The link back url set to the link element.</param>
        /// <param name="twitterUserName">The twitter screen name.</param>
        /// <param name="additionalTweetText">The additional tweet text.</param>
        /// <returns></returns>
        public static HelperResult GetHtml(string pageTitle, string pageLinkBack = null, string twitterUserName = null, string additionalTweetText = null)
        {
            return new HelperResult(delegate(TextWriter __razor_helper_writer)
            {
                var Url = DNA.Utility.UrlUtility.CreateUrlHelper();
                string str;
                if (pageTitle.IsEmpty())
                    throw new ArgumentNullException("pageTitle");

                ConstructPageLinkBack(ref pageLinkBack, out str);
                pageLinkBack = HttpUtility.UrlEncode(pageLinkBack);
                str = HttpUtility.UrlEncode(str);
                pageTitle = HttpUtility.UrlEncode(pageTitle);
                //using (IEnumerator<LinkShareSite> enumerator = GetSitesInOrder(linkSites).GetEnumerator())
                //{
                //    while (enumerator.MoveNext())
                //    {
                //        switch (enumerator.Current)
                //        {
                //            case LinkShareSite.Delicious:
                HelperPage.WriteLiteralTo(__razor_helper_writer, "<a href=\"http://delicious.com/save?v=5&amp;noui&amp;jump=close&amp;url=");
                HelperPage.WriteTo(__razor_helper_writer, str);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;title=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Add to del.icio.us\">\r\n  <img alt=\"Add to del.icio.us\" src=\"" + Url.Content("~/content/images/icon_delicious_16.png") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Add to del.icio.us\" />\r\n                </a>\r\n");
                //   break;

                //  case LinkShareSite.Digg:
                HelperPage.WriteLiteralTo(__razor_helper_writer, "<a href=\"http://digg.com/submit?url=");
                HelperPage.WriteTo(__razor_helper_writer, pageLinkBack);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;title=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Digg!\">\r\n <img alt=\"Digg!\" src=\"" + Url.Content("~/content/images/icon_digg_16.gif") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Digg!\" />\r\n                </a>\r\n");
                //      break;

                //    case LinkShareSite.GoogleBuzz:
                HelperPage.WriteLiteralTo(__razor_helper_writer, " <a href=\"http://www.google.com/reader/link?url=");
                HelperPage.WriteTo(__razor_helper_writer, str);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;title=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Share on Google Buzz\">\r\n <img alt=\"Share on Google Buzz\" src=\"" + Url.Content("~/content/images/icon_google_buzz_16.jpg") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Share on Google Buzz\" />\r\n                </a>\r\n");
                //       break;

                //    case LinkShareSite.Facebook:
                HelperPage.WriteLiteralTo(__razor_helper_writer, "<a href=\"http://www.facebook.com/sharer.php?u=");
                HelperPage.WriteTo(__razor_helper_writer, str);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;t=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Share on Facebook\">\r\n <img alt=\"Share on Facebook\" src=\"" + Url.Content("~/content/images/ico_facebook_16.ico") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Share on Facebook\" />\r\n                </a>\r\n");
                //      break;

                //     case LinkShareSite.Reddit:
                HelperPage.WriteLiteralTo(__razor_helper_writer, "<a href=\"http://reddit.com/submit?url=");
                HelperPage.WriteTo(__razor_helper_writer, pageLinkBack);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;title=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Reddit!\">\r\n <img alt=\"Reddit!\" src=\"" + Url.Content("~/content/images/ico_reddit.ico") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Reddit!\" />\r\n                </a>\r\n");
                //      break;

                //     case LinkShareSite.StumbleUpon:
                HelperPage.WriteLiteralTo(__razor_helper_writer, "<a href=\"http://www.stumbleupon.com/submit?url=");
                HelperPage.WriteTo(__razor_helper_writer, pageLinkBack);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "&amp;title=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Stumble it!\">\r\n <img alt=\"Stumble it!\" src=\"" + Url.Content("~/content/images/icon_su_round_16.gif") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Stumble it!\" />\r\n                </a>\r\n");
                //       break;

                //      case LinkShareSite.Twitter:

                string str2 = string.Empty;
                if (!twitterUserName.IsEmpty())
                {
                    str2 = str2 + ", (via @@" + twitterUserName + ")";
                }
                if (!additionalTweetText.IsEmpty())
                {
                    str2 = str2 + ' ' + additionalTweetText;
                }
                str2 = HttpUtility.UrlEncode(str2);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "                <a href=\"http://twitter.com/home/?status=");
                HelperPage.WriteTo(__razor_helper_writer, pageTitle);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "%3a+");
                HelperPage.WriteTo(__razor_helper_writer, str);
                HelperPage.WriteTo(__razor_helper_writer, str2);
                HelperPage.WriteLiteralTo(__razor_helper_writer, "\" target=\"_blank\" title=\"Share on Twitter\">\r\n                    <img alt=\"Share on Twitter\" src=\"" + Url.Content("~/content/images/ico_twitter_16.ico") + "\" style=\"border:0; height:16px; width:16px; margin:0 1px;\" title=\"Share on Twitter\" />\r\n                </a>\r\n");
                //           break;

                //     }
                //}
                //}
            });

        }

        public static string BitlyApiKey
        {
            get
            {
                return (ScopeStorage.CurrentScope[_bitlyApiKey] as string);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                ScopeStorage.CurrentScope[_bitlyApiKey] = value;
            }
        }

        public static string BitlyLogin
        {
            get
            {
                return (ScopeStorage.CurrentScope[_bitlyLogin] as string);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                ScopeStorage.CurrentScope[_bitlyLogin] = value;
            }
        }

        #region private methods

        private static string GetShortenedUrl(string pageLinkBack)
        {
            if (!BitlyLogin.IsEmpty() && !BitlyApiKey.IsEmpty())
            {
                string str = HttpUtility.UrlEncode(pageLinkBack);
                string key = "Bitly_pageLinkBack_" + BitlyApiKey + "_" + str;
                string webResponse = WebCache.Get(key) as string;
                if (webResponse != null)
                {
                    return webResponse;
                }
                string address = "http://api.bit.ly/v3/shorten?format=txt&longUrl=" + str + "&login=" + BitlyLogin + "&apiKey=" + BitlyApiKey;
                try
                {
                    webResponse = GetWebResponse(address);
                }
                catch (WebException)
                {
                    return pageLinkBack;
                }
                if (webResponse != null)
                {
                    WebCache.Set(key, webResponse, 20, true);
                    return webResponse;
                }
            }
            return pageLinkBack;
        }

        internal static readonly object _bitlyApiKey;

        internal static readonly object _bitlyLogin;

        private static string GetWebResponse(string address)
        {
            string str;
            WebRequest request = WebRequest.Create(address);
            request.Method = "GET";
            request.Timeout = 0x1388;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    str = null;
                }
                else
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (MemoryStream stream2 = new MemoryStream())
                        {
                            stream.CopyTo(stream2);
                            str = Encoding.UTF8.GetString(stream2.ToArray());
                        }
                    }
                }
            }
            return str;
        }

        private static void ConstructPageLinkBack(ref string pageLinkBack, out string shortenedUrl)
        {
            HttpContext current = HttpContext.Current;
            if ((pageLinkBack == null) && (current != null))
            {
                pageLinkBack = current.Request.Url.GetComponents(UriComponents.Path | UriComponents.SchemeAndServer, UriFormat.Unescaped);
            }
            shortenedUrl = GetShortenedUrl(pageLinkBack);
        }

        #endregion
    }
}
