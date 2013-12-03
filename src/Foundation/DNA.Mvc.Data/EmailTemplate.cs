//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    /// <summary>
    /// Represents an email template.
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Gets/Sets the ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets/Sets the email tempalte name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the display title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the language locale name.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets/Sets the email template body.
        /// </summary>
        public string Body { get; set; }
    }
}
