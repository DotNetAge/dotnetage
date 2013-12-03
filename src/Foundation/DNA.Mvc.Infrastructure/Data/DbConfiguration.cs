//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Data.Entity
{
    /// <summary>
    /// Represents a configuration class use to config the entity framework settings.
    /// </summary>
    public class DbConfiguration : IConfiguration
    {
        public bool AutoDetectChangesEnabled { get; set; }
        public bool LazyLoadingEnabled { get; set; }
        public bool ProxyCreationEnabled { get; set; }
        public bool ValidateOnSaveEnabled { get; set; }
    }
}
