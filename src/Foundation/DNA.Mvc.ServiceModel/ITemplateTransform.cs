//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the tempalte transform interface that use to transform the input template to other format
    /// </summary>
    public interface ITemplateTransform
    {
        /// <summary>
        /// Gets the template content type.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Transform the content view template.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="view"></param>
        void Transform(string text,ContentView view);

        /// <summary>
        /// Transform the content form template.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="form"></param>
        void Transform(string text, ContentForm form);
    }
}
