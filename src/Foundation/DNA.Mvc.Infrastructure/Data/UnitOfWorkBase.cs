//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DNA.Data.Entity
{
    public abstract class UnitOfWorksBase<TDBContext> : IUnitOfWorks
        where TDBContext :DbContext
    {
        protected TDBContext dbContext;

        private IDictionary<Type,object> repositoryTable = new Dictionary<Type,object>();
        
        private IRepository<T> GetRepository<T>()
            where T:class
        {
            IRepository<T> repository = null;
            var key=typeof(T);
            
            if (repositoryTable.ContainsKey(key))
                repository = (IRepository<T>)repositoryTable[key];
            else
            {
                repository = GenericRepository<T>();
                repositoryTable.Add(key, repository);
            }

            return repository;
        }

        protected virtual IRepository<T> GenericRepository<T>() where T : class
        {
            return new GenericEntityRepository<T>(dbContext);
        }

        public T Find<T>(object id) where T : class
        {
            return GetRepository<T>().Find(id);
        }

        public T Add<T>(T t) where T : class
        {
            return GetRepository<T>().Create(t);
        }

        public IEnumerable<T> Add<T>(IEnumerable<T> items) where T : class
        {
            var list = new List<T>();
            foreach (var item in items)
                list.Add(Add(item));
            return list;
        }

        public void Update<T>(T t) where T : class
        {
            GetRepository<T>().Update(t);
        }

        public void Delete<T>(T t) where T : class
        {
            GetRepository<T>().Delete(t);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            GetRepository<T>().Delete(predicate);
        }

        public int SaveChanges(bool validateOnSave = true)
        {
            if (!validateOnSave)
                dbContext.Configuration.ValidateOnSaveEnabled = false;

            return dbContext.SaveChanges();
        }

        public void Dispose()
        {
            if (dbContext != null)
                dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
        
        public System.Linq.IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            where T:class
        {
            return GetRepository<T>().Filter(predicate);
        }

        public T Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return GetRepository<T>().Find(predicate);
        }

        public System.Linq.IQueryable<T> All<T>() where T : class
        {
            return GetRepository<T>().All();
        }

        public int Count<T>() where T : class
        {
            return GetRepository<T>().Count();
        }

        public int Count<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return GetRepository<T>().Count(predicate);
        }

        public void Config(IConfiguration settings)
        { 
            var configuration=settings as DbConfiguration ;
            if (configuration != null)
            {
                this.dbContext.Configuration.AutoDetectChangesEnabled = configuration.AutoDetectChangesEnabled;
                this.dbContext.Configuration.LazyLoadingEnabled = configuration.LazyLoadingEnabled;
                this.dbContext.Configuration.ProxyCreationEnabled = configuration.ProxyCreationEnabled;
                this.dbContext.Configuration.ValidateOnSaveEnabled = configuration.ValidateOnSaveEnabled;
            }
        }

        public void Clear<T>() where T : class
        {
            GetRepository<T>().Clear();
        }

        int IUnitOfWorks.SaveChanges()
        {
            return this.SaveChanges();
        }

        public IQueryable<T> All<T>(out int total, int index, int size) where T : class
        {
            return GetRepository<T>().All(out total,index,size);
        }
    
    }
}
