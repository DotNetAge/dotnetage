//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    public class SearchQuery
    {
        public string Terms { get; set; }

        public int Index { get; set; }

        public int Size { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                if (Size == 0)
                    return 0;
                return (int)Math.Ceiling((decimal)TotalItems / (decimal)Size);
            }
        }

        public string Locale { get; set; }

        /// <summary>
        /// Specified search data source
        /// </summary>
        public string Source { get; set; }
    }
}
