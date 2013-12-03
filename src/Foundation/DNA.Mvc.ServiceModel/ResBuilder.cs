//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.IO;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a resource builder.
    /// </summary>
    public class ResBuilder
    {
        /// <summary>
        /// Build .resx file to .resource
        /// </summary>
        /// <param name="input">The input resx file.</param>
        /// <param name="output">The output resource file.</param>
        public void Build(string input, string output)
        {
            var resxs = Directory.GetFiles(input, "*.resx");
            foreach (var resxFile in resxs)
            {
                var binFile = Path.GetDirectoryName(resxFile)+Path.GetFileNameWithoutExtension(resxFile)+".resource";

            }
        }
    }
}
