//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    /// <summary>
    /// Represent a content action use to handle the content event.
    /// </summary>
    public class ContentAction
    {
        /// <summary>
        /// Gets/Sets the content action id.
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// Gets/Sets the parent list id.
        /// </summary>
        public virtual int ParentID { get; set; }

        /// <summary>
        /// Gets/Sets the title text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the description of the action.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets/Sets the event name which action to handle.
        /// </summary>
        public virtual string EventName { get; set; }

        /// <summary>
        /// Gets/Sets the parameter value xml.
        /// </summary>
        public virtual string ParametersXml { get; set; }

        /// <summary>
        /// Gets/Sets the action command assembly qualified name.
        /// </summary>
        public virtual string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/Sets the action execution order.
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Gets/Sets the parent list.
        /// </summary>
        public virtual ContentList Parent { get; set; }
    }
}
