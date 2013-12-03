//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DNA.Data.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public abstract class EntityRepositoryBase<TContext, TObject> : IRepository<TObject>
        where TContext : DbContext
        where TObject : class
    {
        protected TContext context;
        protected DbSet<TObject> dbSet;
        protected bool IsOwnContext = false;

        /// <summary>
        /// Gets the data context object.
        /// </summary>
        protected virtual TContext Context { get { return context; } }

        /// <summary>
        /// Gets the current DbSet object.
        /// </summary>
        protected virtual DbSet<TObject> DbSet { get { return dbSet; } }

        /// <summary>
        /// Dispose the class.
        /// </summary>
        public void Dispose()
        {
            if ((IsOwnContext) && (Context != null))
                Context.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get all objects.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TObject> All()
        {
            return  DbSet.AsQueryable();
        }

        /// <summary>
        /// Gets paging objects.
        /// </summary>
        /// <param name="total">returns the total objects count.</param>
        /// <param name="index">The page index.</param>
        /// <param name="size">The paging size.</param>
        /// <returns>return an object collection.</returns>
        public virtual IQueryable<TObject> All(out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resultSet = skipCount == 0 ? DbSet.Take(size) : DbSet.Skip(skipCount).Take(size);
            total = _resultSet.Count();
            return _resultSet;
        }

        /// <summary>
        /// Gets objects by specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate object.</param>
        /// <returns>return an object collection result.</returns>
        public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> predicate)
        {
            return  DbSet.Where(predicate).AsQueryable<TObject>();
        }

        public virtual IQueryable<TObject> Filter<Key>(Expression<Func<TObject, Key>> sortingSelector, Expression<Func<TObject, bool>> filter, out int total, SortingOrders sortby = SortingOrders.Asc, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resultSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();
            total = _resultSet.Count();
            _resultSet = sortby == SortingOrders.Asc ? _resultSet.OrderBy(sortingSelector).AsQueryable() : _resultSet.OrderByDescending(sortingSelector).AsQueryable();
            _resultSet = skipCount == 0 ? _resultSet.Take(size) : _resultSet.Skip(skipCount).Take(size);
            return _resultSet;
        }

        public bool Contains(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual TObject Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual TObject Find(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
            //return _isNoTracking ? DbSet.AsNoTracking().FirstOrDefault(predicate) : DbSet.FirstOrDefault(predicate);
        }

        public virtual TObject Create(TObject TObject)
        {
            var newEntry = DbSet.Add(TObject);
            if (IsOwnContext)
                Context.SaveChanges();
            return newEntry;
        }

        public virtual void Delete(TObject TObject)
        {
            var entry = Context.Entry(TObject);
            DbSet.Remove(TObject);
            if (IsOwnContext)
                Context.SaveChanges();
        }

        public virtual TObject Update(TObject TObject)
        {
            var entry = Context.Entry(TObject);
            DbSet.Attach(TObject);
            entry.State = EntityState.Modified;
            if (IsOwnContext)
                Context.SaveChanges();
            return TObject;
        }

        public virtual int Delete(Expression<Func<TObject, bool>> predicate)
        {
            var objects = DbSet.Where(predicate).ToList();
            foreach (var obj in objects)
                DbSet.Remove(obj);
            
            if (IsOwnContext)
                return Context.SaveChanges();
            return objects.Count();
        }

        public virtual int Count()
        {
            return DbSet.Count();
        }

        public virtual int Count(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public int Submit()
        {
            return Context.SaveChanges();
        }

        //#region Implement IConfigurable
        //bool IConfigurable.AutoDetectChangesEnabled
        //{
        //    get
        //    {
        //        return this.Context.Configuration.AutoDetectChangesEnabled;
        //    }
        //    set
        //    {
        //        this.Context.Configuration.AutoDetectChangesEnabled = value;
        //    }
        //}

        //bool IConfigurable.LazyLoadingEnabled
        //{
        //    get
        //    {
        //        return this.Context.Configuration.LazyLoadingEnabled;
        //    }
        //    set
        //    {
        //        this.Context.Configuration.LazyLoadingEnabled = value;
        //    }
        //}

        //bool IConfigurable.ProxyCreationEnabled
        //{
        //    get
        //    {
        //        return this.Context.Configuration.ProxyCreationEnabled;
        //    }
        //    set
        //    {
        //        this.Context.Configuration.ProxyCreationEnabled = value;
        //    }
        //}

        //bool IConfigurable.ValidateOnSaveEnabled
        //{
        //    get
        //    {
        //        return this.Context.Configuration.ValidateOnSaveEnabled;
        //    }
        //    set
        //    {
        //        this.Context.Configuration.ValidateOnSaveEnabled = value;
        //    }
        //}
        //#endregion

        public virtual void Clear()
        {
             
        }
    }
}
