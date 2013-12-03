//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent an attachment class of ContentDataItem
    /// </summary>
    public class ContentAttachment
    {
        /// <summary>
        /// Gets/Sets the ID.
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file url.
        /// </summary>
        public virtual string Uri { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file content type.
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file extension name.
        /// </summary>
        public virtual string Extension { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file size.
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// Gets/Sets the download times of the attachment file.
        /// </summary>
        public virtual int Downloads { get; set; }

        /// <summary>
        /// Gets/Sets the attach data item id.
        /// </summary>
        public virtual Guid ItemID { get; set; }

        /// <summary>
        /// Gets/Sets the attach data item version.
        /// </summary>
        public virtual int ItemVersion { get; set; }

        /// <summary>
        /// Gets/Sets the attachment file privacy value.
        /// </summary>
        public virtual int Privacy { get; set; }

        /// <summary>
        /// Gets/Sets the attach ContentDataItem objec.t
        /// </summary>
        public virtual ContentDataItem Item { get; set; }
    }
}
