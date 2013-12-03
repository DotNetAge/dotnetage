//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data.Documents;
using DNA.Web.Logging;
using System;
using System.Text;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represent the 
    /// </summary>
    [Inject("ILogger")]
    public class DefaultLogger : ILogger
    {
        public string DataPath { get; set; }

        private void Write(string message, LogEntityTypes type, Exception e)
        {
            var basePath = "~/app_data/logs";
            var directory = System.Web.Hosting.HostingEnvironment.MapPath(basePath);
            using (var blobs = new DocumentStorage(directory))
            {
                var entity = new LogEntity()
                {
                    Message =message,
                    LogEntityType = type,
                    Logged = DateTime.UtcNow
                };

                if (e != null)
                {
                    var detailLines = new StringBuilder();
                    detailLines.AppendLine("Source error:" + e.Source);
                    detailLines.Append("Stack Trace:" + e.StackTrace);
                    var innerExp = e.InnerException;
                    if (innerExp != null)
                    {
                        detailLines.AppendLine("Inner exception:" + innerExp.Message);

                        while (innerExp != null)
                        {
                            detailLines.AppendLine("Message:" + innerExp.Source);
                            detailLines.AppendLine("Source error:" + e.Source);
                            detailLines.Append("Stack Trace:" + e.StackTrace);
                            innerExp = innerExp.InnerException;
                        }
                    }
                }

                blobs.Add(entity);
                blobs.SaveChanges();
            }

        }

        public void Info(string message)
        {
            Write(message, LogEntityTypes.Info, null);
        }

        public void Warn(string message)
        {
            Write(message, LogEntityTypes.Warning, null);
        }

        public void Error(Exception e,string message)
        {
            Write(message, LogEntityTypes.Error, e);
        }

        public void Fatal(Exception e,string message)
        {
            Write(message, LogEntityTypes.Fatal, e);
        }
    }
}
