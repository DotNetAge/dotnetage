//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Utility
{
    public class XmlDeserializeException:Exception
    {
        string _file = "";
        string _type = "";

        public XmlDeserializeException(string fileName, string typeString)
        {
            _file = fileName;
            _type = typeString;
        }

        public override string Message
        {
            get
            {
                return "Deserialize object from \""+_file+"\" as type \""+_type+"\" fail.Mybe the xml document no correct.";
            }
        }
    }
}
