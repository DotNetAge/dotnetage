//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DNA.Web
{
    /// <summary>
    /// Represent a choice filed
    /// </summary>
    public class ChoiceField : ContentField
    {
        public const string MULTIPLE = "multiple";
        public const string FILL = "fill";
        public const string CHOCIES = "choices";
        public const string FORMAT = "format";
        public const string OPTION = "option";
        public const string VALUE = "value";

        /// <summary>
        ///  Initializes a new instance of the ChoiceField.
        /// </summary>
        public ChoiceField() { FieldType = 3; }

        /// <summary>
        /// Gets/Sets the field can be fill input value.
        /// </summary>
        public bool FillInChoice { get; set; }

        /// <summary>
        /// Gets/Sets the field can be select more then one choice values.
        /// </summary>
        public bool AllowMultiChoice { get; set; }

        /// <summary>
        /// Gets/Sets the drop dropwn choice's display style the possible value is convert the 
        /// </summary>
        public int DropdownFormat { get; set; }

        /// <summary>
        /// Gets/Sets choices raw string
        /// </summary>
        public string Choices { get; set; }

        /// <summary>
        /// Gets/Sets choice values
        /// </summary>
        public string[] ChoiceValues
        {
            get
            {
                if (!string.IsNullOrEmpty(Choices))
                    return Choices.Split(',');
                return null;
            }
            set
            {
                if (value != null)
                    Choices = string.Join(",", value);
                else
                    Choices = "";
            }
        }

        /// <summary>
        /// Save ChoiceField properties to xml.
        /// </summary>
        /// <param name="element">The xml element</param>
        protected override void SaveTo(XElement element)
        {
            element.Add(new XAttribute("type", "Choice"));

            if (this.DefaultValue != null)
                element.Add(new XAttribute(DEFAULT, this.DefaultValue.ToString()));

            XNamespace ns = ContentList.DefaultNamespace;

            var choiceElement = new XElement(ns + CHOCIES,
                new XAttribute(MULTIPLE, this.AllowMultiChoice),
                new XAttribute(FILL, this.FillInChoice),
                new XAttribute(FORMAT, this.DropdownFormat == 1 ? "DropDownList" : "RadioButtons")
                );

            if (!string.IsNullOrEmpty(Choices))
            {
                var _choices = Choices.Split(',');
                foreach (var _choice in _choices)
                {
                    choiceElement.Add(new XElement(ns + OPTION, new XAttribute(VALUE, _choice) { Value = _choice }));
                }
            }
            element.Add(choiceElement);
        }

        /// <summary>
        /// Load choice field property from xml element
        /// </summary>
        /// <param name="element">The xelement object.</param>
        /// <param name="locale">The locale name</param>
        public override void Load(XElement element, string locale = "")
        {
            var ns = element.GetDefaultNamespace();

            base.Load(element, locale);

            this.FieldType = 3;
            var chocieEle = element.Element(ns + CHOCIES);
            this.DefaultValue = element.StrAttr(DEFAULT);

            if (chocieEle != null)
            {
                this.FillInChoice = chocieEle.BoolAttr(FILL);
                this.AllowMultiChoice = chocieEle.BoolAttr(MULTIPLE);
                this.DropdownFormat = chocieEle.IntAttr(FORMAT);
                var options = chocieEle.Elements(ns + OPTION);

                if (options != null && options.Count() > 0)
                {
                    // this.Choices = new NameValueCollection();
                    var _choices = new List<string>();
                    foreach (var opt in options)
                    {
                        _choices.Add(opt.StrAttr(VALUE));
                    }

                    if (_choices.Count > 0)
                        this.Choices = string.Join(",", _choices.ToArray());
                    else
                        this.Choices = "";
                }
            }

        }
    }


}
