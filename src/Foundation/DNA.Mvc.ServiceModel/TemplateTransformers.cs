//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a helper class to locate ITemplateTransform instance.
    /// </summary>
    public class TemplateTransformers
    {
        /// <summary>
        /// Gets the ITemplateTransform by specified content type.
        /// </summary>
        /// <param name="contentType">The content type name.</param>
        /// <returns></returns>
        public static ITemplateTransform Get(string contentType)
        {
            var transforms= App.GetServices<ITemplateTransform>();
            return transforms.FirstOrDefault(t => t.ContentType.Equals(contentType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets supported content type names.
        /// </summary>
        /// <returns></returns>
        public string[] Supports()
        {
            return App.GetServices<ITemplateTransform>().Select(t => t.ContentType).ToArray();
        }
    }
}
