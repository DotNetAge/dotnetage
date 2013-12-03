//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;
using System.Web;

namespace DNA.Web
{
    /// <summary>
    /// Provide helper methods for http request.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Identity current request sent by mobile device.
        /// </summary>
        /// <param name="request">The http request object</param>
        /// <returns>A boolean value</returns>
        public static bool IsMobileRequest(this HttpRequestBase request)
        {
            //FIRST TRY BUILT IN ASP.NT CHECK
            if (request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (request.ServerVariables["HTTP_ACCEPT"] != null &&
                request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
              {
                  "midp", "j2me", "avant", "docomo",
                  "novarra", "palmos", "palmsource",
                  "240x320", "opwv", "chtml",
                  "pda", "windows ce", "mmp/",
                  "blackberry", "mib/", "symbian",
                  "wireless", "nokia", "hand", "mobi",
                  "phone", "cdm", "up.b", "audio",
                  "SIE-", "SEC-", "samsung", "HTC",
                  "mot-", "mitsu", "sagem", "sony"
                  , "alcatel", "lg", "eric", "vx",
                  "NEC", "philips", "mmm", "xx",
                  "panasonic", "sharp", "wap", "sch",
                  "rover", "pocket", "benq", "java",
                  "pt", "pg", "vox", "amoi",
                  "bird", "compal", "kg", "voda",
                  "sany", "kdd", "dbt", "sendo",
                  "sgh", "gradi", "jb", "dddi",
                  "moto", "iphone"
              };

                //Loop through each item in the list created above
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains(s.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get website name from current request.
        /// </summary>
        /// <param name="request">The http request object</param>
        /// <returns>A string value of the website name.</returns>
        public static string GetWebName(this HttpRequestBase request)
        {
            string name = App.Get().Context.Website;
            return string.IsNullOrEmpty(name) ? "home" : name;
        }
    }
}
