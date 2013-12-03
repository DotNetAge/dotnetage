//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)


using System.Data.Entity;
using DNA.Data.Entity;

namespace DNA.Web.Data.Entity
{
    public class EntityRepository<T> : EntityRepositoryBase<CoreDbContext, T>
        where T : class
    {
        public EntityRepository()
        {
            context = new CoreDbContext();
            dbSet = this.Context.Set<T>();
            IsOwnContext = true;
        }

        public EntityRepository(CoreDbContext dbContext)
        {
            context = dbContext;
            dbSet = this.Context.Set<T>();
            IsOwnContext = false;
        }
    }
}
