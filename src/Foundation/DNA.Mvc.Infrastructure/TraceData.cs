//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA
{
    /// <summary>
    /// Presents the trace data object.
    /// </summary>
    [Obsolete]
    public class TraceData
    {
        public TraceData(string message)
        {
            Category = "Info";
            Time = DateTime.Now;
            Message = message;
        }

        /// <summary>
        /// Gets the trace time
        /// </summary>
        public virtual DateTime Time { get; set; }

        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual string Category { get; set; }

        /// <summary>
        /// Gets the trace detail message text.
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Gets whether this is a warnning message.
        /// </summary>
        public virtual bool IsWarn { get; set; }

        public virtual bool IsError { get { return ErrorInfo != null; } }
        
        /// <summary>
        /// Gets the ErrorInfo object.
        /// </summary>
        public virtual Exception ErrorInfo { get; set; }
    }
}
