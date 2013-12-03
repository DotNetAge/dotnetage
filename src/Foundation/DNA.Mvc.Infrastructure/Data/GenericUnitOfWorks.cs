//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Data.Entity;

namespace DNA.Data.Entity
{
    public class GenericUnitOfWorks<TDbContext> : UnitOfWorksBase<TDbContext>
        where TDbContext : DbContext
    {
        public GenericUnitOfWorks()
        {
            this.dbContext = Activator.CreateInstance<TDbContext>();
        }

        public GenericUnitOfWorks(TDbContext context) { this.dbContext = context; }
    }
}
