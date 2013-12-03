//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.ServiceModel;

namespace DNA.Web.Events
{
    /// <summary>
    /// Represents an event argument object that raise on a new page created.
    /// </summary>
    public class WebPageEventArgs
    {
        /// <summary>
        /// Gets the created WebPage object.
        /// </summary>
        public WebPageDecorator Page { get; set; }
    }

    /// <summary>
    /// Represents a web page deleted event arugment class.
    /// </summary>
    public class WebPageDeletedEventArgs
    {
        /// <summary>
        /// Gets the deleted page id.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the deleted page path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the deleted page locale
        /// </summary>
        public string Locale { get; private set; }

        /// <summary>
        /// Gets the deleted page parent web object.
        /// </summary>
        public WebDecorator Web { get; private set; }

    }
}
