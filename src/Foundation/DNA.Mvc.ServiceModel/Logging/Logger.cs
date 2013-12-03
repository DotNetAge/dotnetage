//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Logging;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DNA.Web
{
    public class Logger
    {
        private static IEnumerable<ILogger> Loggers
        {
            get
            {
                return DependencyResolver.Current.GetServices<ILogger>();
            }
        }

        public static void Info(string message)
        {
            foreach (var log in Loggers)
            {
                try
                {
                    log.Info(message);
                }
                catch (Exception) { continue; }
            }
        }

        public static void Warn(string message)
        {
            foreach (var log in Loggers)
            {
                try
                {
                    log.Warn(message);
                }
                catch (Exception) { continue; }
            }
        }

        public static void Error(Exception e)
        {
            Error(e.Message, e);
        }

        public static void Error(string message, Exception e)
        {
            foreach (var log in Loggers)
            {
                try
                {
                    log.Error(e, message);
                }
                catch (Exception) { continue; }
            }
        }

        public static void Fatal(Exception e)
        {
            Fatal(e.Message, e);
        }

        public static void Fatal(string message, Exception e)
        {
            foreach (var log in Loggers)
            {
                try
                {
                    log.Fatal(e, message);
                }
                catch (Exception) { continue; }
            }
        }
    }
}
