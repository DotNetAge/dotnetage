//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Serialization;

namespace DNA.Xml
{
    public class ScriptElement : ResElement
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        public new ScriptElement Clone()
        {
            var copy = new ScriptElement()
            {
                Source = this.Source,
                Text = this.Text,
                Type = this.Type
            };
            return copy;
        }

    }
}
