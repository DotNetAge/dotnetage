///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.Data.Entity
{
    public class WebPageIsExistsException : Exception
    {
        public WebPageIsExistsException() : base(Properties.Resources.WebPageIsExistsException_Msg) { }
        public WebPageIsExistsException(string msg) : base(Properties.Resources.WebPageIsExistsException_Msg + " " + msg) { }
    }
}
