//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DNA.Utility
{
    public class FileUtility
    {
        public static byte[] ReadStream(Stream stream)
        {
            try
            {
                stream.Position = 0;
            }
            catch
            {
            }

            byte[] readBuffer = new byte[1024];
            List<byte> outputBytes = new List<byte>();

            int offset = 0;
            while (true)
            {
                int bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                else if (bytesRead == readBuffer.Length)
                {
                    outputBytes.AddRange(readBuffer);
                }
                else
                {
                    byte[] tempBuf = new byte[bytesRead];
                    Array.Copy(readBuffer, tempBuf, bytesRead);
                    outputBytes.AddRange(tempBuf);
                    break;
                }
                offset += bytesRead;
            }
            return outputBytes.ToArray();
        }

        public static string GetContentType(string fileName)
        {
            //string mime = "application/octetstream";
            //Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(fileName));
            //if (rk != null && rk.GetValue("Content Type") != null)
            //    mime = rk.GetValue("Content Type").ToString();
            //return mime;
            var ext = Path.GetExtension(fileName);
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
            //mimes.Add(".js", "application/x-javascript");
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
            mimes.Add(".css", "text/plain");
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

            if (mimes.ContainsKey(ext))
                return mimes[ext];
            else
                return mime;
        }

        public static void CopyDirectory(string Src, string Dst)
        {
            String[] Files;

            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);

            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;

            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                // Sub directories

                if (Directory.Exists(Element))
                    CopyDirectory(Element, Dst + Path.GetFileName(Element));
                // Files in directory

                else
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
            }
        }

        public static bool CheckAccessRight(string path, FileSystemRights right)
        {
            if (Path.HasExtension(path))
            {
                var dirInfo = new FileInfo(path);
                return CheckAccessRight(dirInfo, right);
            }
            else
            {
                var dirInfo = new DirectoryInfo(path);
                return CheckAccessRight(dirInfo, right);
            }
        }

        public static bool CheckAccessRight(FileInfo file, FileSystemRights right)
        {
            var user = WindowsIdentity.GetCurrent();
            var p = new WindowsPrincipal(user);
            AuthorizationRuleCollection acl =
            file.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            return CheckAccessRight(user, p, acl, right);
        }
        public static bool CheckAccessRight(DirectoryInfo directory, FileSystemRights right)
        {
            var user = WindowsIdentity.GetCurrent();
            var p = new WindowsPrincipal(user);
            AuthorizationRuleCollection acl =
            directory.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            return CheckAccessRight(user, p, acl, right);
        }

        public static bool CheckAccessRight(WindowsIdentity user, WindowsPrincipal principal, AuthorizationRuleCollection acl, FileSystemRights right)
        {
            // Get the collection of authorization rules that apply to the current directory
            //AuthorizationRuleCollection acl =
            //directory.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            // These are set to true if either the allow or deny  access rights are set
            bool allow = false;
            bool deny = false;

            for (int x = 0; x < acl.Count; x++)
            {
                FileSystemAccessRule currentRule = (FileSystemAccessRule)acl[x];
                // If the current rule applies to the current user
                if (user.User.Equals(currentRule.IdentityReference) || principal.IsInRole((SecurityIdentifier)currentRule.IdentityReference))
                {
                    if
                    (currentRule.AccessControlType.Equals(AccessControlType.Deny))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            deny = true;
                        }
                    }
                    else if
                    (currentRule.AccessControlType.Equals(AccessControlType.Allow))
                    {
                        if ((currentRule.FileSystemRights & right) == right)
                        {
                            allow = true;
                        }
                    }
                }
            }

            if (allow & !deny)
                return true;
            else
                return false;
        }

    }
}