//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    [Obsolete]
    public class WebPageVersion
    {
        public int ID { get; set; }

        public DateTime Published { get; set; }

        public int Version { get; set; }

        public string Remarks { get; set; }

        public string Content { get; set; }
        
        public int PageID { get; set; }

        public virtual WebPage Page { get; set; }
    }
}
