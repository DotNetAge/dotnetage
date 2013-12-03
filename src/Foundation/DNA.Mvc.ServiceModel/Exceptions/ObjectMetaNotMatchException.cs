//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web.ServiceModel
{
    public class ObjectMetaNotMatchException : Exception
    {
        private string message = "";

        public ObjectMetaNotMatchException() { }
        public ObjectMetaNotMatchException(string msg) { this.message = msg; }

        public override string Message
        {
            get
            {
                return message;
            }
        }
    }
}
