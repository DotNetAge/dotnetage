//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    [Flags,Obsolete]
    public enum TrackStates
    {
        NoChange = 0,
        Modified = 1,
        Deleted = 2,
        Added = 3
    }
}
