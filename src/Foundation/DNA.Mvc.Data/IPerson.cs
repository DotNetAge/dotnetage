//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;

namespace DNA.Web
{
    /// <summary>
    /// Define a person base properties
    /// </summary>
    public interface IPerson
    {
        /// <summary>
        /// Gets/Sets the birthday
        /// </summary>
        DateTime? Birthday { get; set; }

        /// <summary>
        /// Gets/Sets the photo url.
        /// </summary>
        string Photo { get; set; }

        /// <summary>
        /// Gets/Sets the first name.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Gets/Sets the last name.
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Gets/Sets the middle name.
        /// </summary>
        string MiddleName { get; set; }

        /// <summary>
        /// Gets / Sets the gender.
        /// </summary>
        string Gender { get; set; }

        /// <summary>
        /// Gets/Sets the description of the person.
        /// </summary>
        string About { get; set; }

        /// <summary>
        /// Gets/Sets the tax id.
        /// </summary>
        string TaxID { get; set; }

        /// <summary>
        /// Gets/Sets the person currency code.
        /// </summary>
        string CurrencyCode { get; set; }
    }
}
