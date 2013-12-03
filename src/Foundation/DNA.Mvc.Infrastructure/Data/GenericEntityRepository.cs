//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System.Data.Entity;

namespace DNA.Data.Entity
{
    public class GenericEntityRepository<T> : EntityRepositoryBase<DbContext, T>
        where T:class
    {
        public GenericEntityRepository(DbContext dbContext)
        {
            context = dbContext;
            dbSet = this.Context.Set<T>();
            IsOwnContext = false;
        }
    }
}
