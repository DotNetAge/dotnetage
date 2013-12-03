//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Represent a exception class indiests the content field object not found.
    /// </summary>
    public class ContentFieldNotFoundException:Exception
    {
        private string message = "Field not found";
        
        public string FieldName { get; private set; }

        public ContentFieldNotFoundException(string fieldName)
        {
            message = string.Format("Can not found the \"{0}\" field definition in list.", fieldName);
            FieldName = fieldName;
        }

        public override string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}
