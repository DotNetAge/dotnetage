//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.Logging
{
    /// <summary>
    /// Represent the entity for log
    /// </summary>
    public class LogEntity
    {
        /// <summary>
        /// Gets/Sets the log id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the entity type.
        /// </summary>
        public LogEntityTypes LogEntityType{get;set;}

        /// <summary>
        /// Gets/Sets the log message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets/Sets the error/fatal message detail.
        /// </summary>
        public string MessageDetail { get; set; }

        /// <summary>
        /// Get/sets the message write time.
        /// </summary>
        public DateTime Logged { get; set; }
    }
}
