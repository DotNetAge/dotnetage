///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.Data.Entity
{
    public class WebSiteIsExistsException:Exception
    {
        public WebSiteIsExistsException() : base(Properties.Resources.WebSiteIsExistsException_Msg) { }
        public WebSiteIsExistsException(string msg) : base(Properties.Resources.WebSiteIsExistsException_Msg+" "+msg) { }
    }
}
