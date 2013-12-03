//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.ServiceModel.Transformers
{
    public class jQueryTemplateTransformer:ITemplateTransform
    {
        public string ContentType
        {
            get { return TemplateContentTypes.jQuery; }
        }

        public void Transform(string text, ContentView view)
        {
            throw new NotImplementedException();
        }

        public void Transform(string text, ContentForm form)
        {
            throw new NotImplementedException();
        }
    }
}
