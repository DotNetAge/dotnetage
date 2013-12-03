//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web
{
    /// <summary>
    /// Represent a state province class define the region info.
    /// </summary>
    public class StateProvince
    {
        public StateProvince() { }

        public StateProvince(string code, string englishName) { this.Code = code; this.EnglishName = englishName; this.NativeName = englishName; }

        public StateProvince(string code, string englishName, string nativeName) : this(code, englishName) { this.NativeName = nativeName; }

        /// <summary>
        /// Gets/Sets three ISO letters country code.
        /// </summary>
        public string CountryCode { get; set; }

        public string EnglishName { get; set; }

        public string NativeName { get; set; }

        public string Code { get; set; }
    }
}
