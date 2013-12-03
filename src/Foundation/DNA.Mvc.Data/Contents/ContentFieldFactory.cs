//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Xml.Linq;

namespace DNA.Web.Contents
{
    /// <summary>
    /// Represents a content field factory use to create field instance.
    /// </summary>
    public class ContentFieldFactory
    {
        /// <summary>
        /// Create field by specified type id.
        /// </summary>
        /// <param name="typeID">The field type id.</param>
        /// <returns>A new field instance returns.</returns>
        public ContentField Create(int typeID)
        {
            switch (typeID)
            {
                case 2://ContentFieldTypes.Note:
                    return new NoteField();
                case 3:  //ContentFieldTypes.Choice:
                    return new ChoiceField();
                case 4://ContentFieldTypes.Integer:
                    return new IntegerField();
                case 5://ContentFieldTypes.Number:
                    return new NumberField();
                case 6://ContentFieldTypes.Computed:
                    return new ComputedField();
                case 7:
                    return new LookupField();
                case 8:   //ContentFieldTypes.Boolean:
                    return new BooleanField();
                case 9:// ContentFieldTypes.DateTime:
                    return new DateField();
                case 10://ContentFieldTypes.Image:
                    return new ImageField();
                case 11://ContentFieldTypes.File:
                    return new FileField();
                case 12: //ContentFieldTypes.Currency:
                    return new CurrencyField();
                case (int)ContentFieldTypes.Video:
                    return new VideoField();
                case (int)ContentFieldTypes.User://ContentFieldTypes.User:
                    return new UserField();
                default:
                    return new TextField();
            }
        }

        /// <summary>
        /// Create field by specified type name.
        /// </summary>
        /// <param name="typeName">The field type name.</param>
        /// <returns>A new field instance returns.</returns>
        public ContentField Create(string typeName)
        {
            if (typeName.Equals("Boolean"))
                return new BooleanField();

            if (typeName.Equals("Choice"))
                return new ChoiceField();

            if (typeName.Equals("Currency"))
                return new CurrencyField();

            if (typeName.Equals("DateTime"))
                return new DateField();

            if (typeName.Equals("File"))
                return new FileField();

            if (typeName.Equals("Image"))
                return new ImageField();

            if (typeName.Equals("User"))
                return new UserField();

            if (typeName.Equals("Integer"))
                return new IntegerField();

            if (typeName.Equals("Number"))
                return new NumberField();

            if (typeName.Equals("Note"))
                return new NoteField();

            if (typeName.Equals("Computed"))
                return new ComputedField();

            if (typeName.Equals("Lookup"))
                return new LookupField();

            if (typeName.Equals("Video"))
                return new VideoField();

            return new TextField();
        }

        /// <summary>
        /// Create a field instance by specified xml element.
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <param name="locale">The locale name.</param>
        /// <returns>A new field instance returns.</returns>
        public ContentField Create(XElement element, string locale = "")
        {
            var field = Create(element.StrAttr("type"));
            field.Load(element, locale);
            return field;
        }
    }
}
