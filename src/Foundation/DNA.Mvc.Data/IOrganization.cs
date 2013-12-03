//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    /// <summary>
    /// Defines an organization base properies
    /// </summary>
    public interface IOrganization
    {
        /// <summary>
        /// Gets/Sets the organization name.
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Gets/Sets the organization legal name.
        /// </summary>
        string LegalName { get; set; }
        
        /// <summary>
        /// Gets/Sets the logo url
        /// </summary>
        string Logo { get; set; }
        
        /// <summary>
        /// Gets/Sets the brand name.
        /// </summary>
        string Brand { get; set; }

        /// <summary>
        /// Gets/Sets the default currency code.
        /// </summary>
        string CurrencyCode { get; set; }

        /// <summary>
        /// Gets/Sets the  taxid of the organization.
        /// </summary>
        string TaxID { get; set; }

        /// <summary>
        /// Gets/Sets the organization description.
        /// </summary>
        string Description { get; set; }

    }
}
