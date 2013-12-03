//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;

namespace DNA.Web
{
    /// <summary>
    /// This class use to read and format the email templates 
    /// </summary>
    public static class EmailTemplateHelper
    {
        /// <summary>
        /// Read the specified email temlpate file and format the value fields in template content.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="formatObject"></param>
        /// <returns></returns>
        public static string Read(string file, object formatObject = null)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            var tmplContent = "";
            if (file.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ||
file.EndsWith(".vbhtml", StringComparison.OrdinalIgnoreCase) ||
file.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                tmplContent = ReadWebPageTemplate(file);
            else
                tmplContent = ReadHtmlTemplate(file);

            if (formatObject != null)
                return Format(tmplContent, formatObject);

            return tmplContent;
        }

        public static string Format(string tmplContent, object formatObject)
        {
            if (formatObject != null)
            {
                var result = tmplContent;
                var valueDict = ConvertToDictionary(formatObject);
                foreach (var key in valueDict.Keys)
                    result = result.Replace("${" + key + "}", (valueDict[key] != null ? valueDict[key].ToString() : ""));
                return result;
            }
            return tmplContent;
        }

        private static string ReadHtmlTemplate(string file)
        {
            string fileName = HttpContext.Current.Server.MapPath(file);

            var bodyHtml = "";
            if (File.Exists(fileName))
            {
                try
                {
                    using (var reader = File.OpenText(fileName))
                    {
                        bodyHtml = reader.ReadToEnd();
                    }
                }
                catch { return ""; }
            }
            return bodyHtml;
        }

        private static string ReadWebPageTemplate(string file)
        {
            var httpRequest = HttpContext.Current.Request;
            string url = httpRequest.Url.Scheme + "://" + httpRequest.Url.Authority;
            if (!httpRequest.ApplicationPath.Equals("/"))
                url += httpRequest.ApplicationPath;

            if (file.StartsWith("~/"))
                url = file.Replace("~", url);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Accept-Encoding"] = "gzip";
            request.Headers["Accept-Language"] = "en-us";
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            var response = request.GetResponse();
            var htmlContent = "";
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                htmlContent = reader.ReadToEnd();
            }
            return htmlContent;
        }

        private static IDictionary<string, object> ConvertToDictionary(object data)
        {
            if (data is IDictionary<string, object>)
                return data as IDictionary<string, object>;

            var attr = BindingFlags.Public | BindingFlags.Instance;
            var dict = new Dictionary<string, object>();
            foreach (var property in data.GetType().GetProperties(attr))
            {
                if (property.CanRead)
                {
                    dict.Add(property.Name, property.GetValue(data, null));
                }
            }
            return dict;
        }
    }
}
