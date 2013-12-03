//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.Logging
{
    /// <summary>
    /// Define the methods for logging.
    /// </summary>
    [Inject]
    public interface ILogger
    {
        /// <summary>
        /// Write the information message to log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Info(string message);

        /// <summary>
        /// Write the warnning message to log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Warn(string message);

        /// <summary>
        /// Write error message to log.
        /// </summary>
        /// <param name="e">The error exception object</param>
        /// <param name="message">The additional error message text.</param>
        void Error(Exception e, string message);

        /// <summary>
        /// Write the fatal message to log.
        /// </summary>
        /// <param name="e">The error exception object</param>
        /// <param name="message">The additional error message text.</param>
        void Fatal(Exception e, string message);

    }

}
