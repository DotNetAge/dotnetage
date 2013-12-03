//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections;
using System.IO;
using System.Resources;

namespace DNA.Web
{
    public class ResBuilder
    {
        /// <summary>
        /// Build .resx file to .resource
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void Build(string input)
        {
            var resxs = Directory.GetFiles(input, "*.resx");
            foreach (var resxFile in resxs)
            {
                var binFile = Path.GetDirectoryName(resxFile) +"\\"+ Path.GetFileNameWithoutExtension(resxFile) + ".resources";
                if (File.Exists(binFile)) {
                    var resxFileInfo = new FileInfo(resxFile);
                    var binFileInfo = new FileInfo(binFile);
                    if (resxFileInfo.LastWriteTime > binFileInfo.LastWriteTime)
                        File.Delete(binFile); //Re-complied
                }

                if (!File.Exists(binFile))
                {
                    using (var reader = new ResXResourceReader(resxFile))
                    {
                        using (var writer = new ResourceWriter(binFile))
                        {
                            foreach (DictionaryEntry d in reader)
                            {
                                writer.AddResource(d.Key.ToString(), d.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
