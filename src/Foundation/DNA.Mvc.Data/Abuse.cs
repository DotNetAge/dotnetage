//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent an abuse 
    /// </summary>
    public class Abuse
    {
        /// <summary>
        /// Gets/Sets the ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the uri which abuse report for.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets/Sets the abuse type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets/Sets the abuse detail description.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets/Sets the object type string.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets/Sets the who abuse the resource.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets/Sets the user name of the reportor.
        /// </summary>
        public string Reportor { get; set; }

        /// <summary>
        /// Indicates whether the abuse is resolved.
        /// </summary>
        public bool IsResolved { get; set; }

        /// <summary>
        /// Gets/Sets the date of report.
        /// </summary>
        public DateTime ReportingDate { get; set; }
    }
}
