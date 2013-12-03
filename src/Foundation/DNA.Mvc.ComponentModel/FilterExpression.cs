///  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
///  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
///  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    public class FilterExpression
    {
        public FilterExpression(string expr)
        {
            var exprArgs = expr.Split('~');
            FieldName = exprArgs[0];
            if (exprArgs.Length > 1)
                Operator = exprArgs[1];
            if (exprArgs.Length>2)
            Term = exprArgs[exprArgs.Length - 1];
        }
        
        /// <summary>
        /// Get/Sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets/Sets the operator name
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets/Sets the match term
        /// </summary>
        public string Term { get; set; }
    }
}
