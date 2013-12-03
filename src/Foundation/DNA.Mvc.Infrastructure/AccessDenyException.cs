//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
   public class AccessDenyException:Exception
    {
       public override string Message
       {
           get
           {
               return "Access denied! You does not have enougth permission to complete this operation.";
               //return base.Message;
           }
       }
    }
}
