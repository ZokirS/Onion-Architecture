﻿using Microsoft.EntityFrameworkCore;
using Contracts;
using System.Linq.Expressions;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T: class
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
         => RepositoryContext = repositoryContext;

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ? RepositoryContext.Set<T>()
            .AsNoTracking() :
            RepositoryContext.Set<T>();



        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        => !trackChanges ?
            RepositoryContext.Set<T>()
            .Where(expression)
            .AsNoTracking() :
            RepositoryContext.Set<T>()
            .Where(expression);

        public void Create(T entity)
        {
           RepositoryContext.Add(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Remove(entity);
        }

        public void Update(T entity)
        {
            RepositoryContext.Update(entity);
        }
    }
}
