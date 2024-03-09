﻿using CONFİDO.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CONFİDO.Data.Repositories
{
   
    
        public class Repository<T> : IRepository<T> where T : class, IEntityBase, new()
        {
            private Context.AppContext dbContext;
            public Repository(Context.AppContext dbContext)
            {
                this.dbContext = dbContext;
            }
            private DbSet<T> Table => dbContext.Set<T>();

            public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
            {
                IQueryable<T> query = Table;
                if (predicate != null)
                    query = query.Where(predicate);

                if (includeProperties.Any())
                    foreach (var item in includeProperties)
                        query = query.Include(item);

                return await query.ToListAsync();
            }
            public async Task AddAsync(T entity)
            {
                await Table.AddAsync(entity);
            }

            public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
            {
                IQueryable<T> query = Table;
                query = query.Where(predicate);

                if (includeProperties.Any())
                    foreach (var item in includeProperties)
                        query = query.Include(item);

                return await query.SingleAsync();
            }

            public async Task<T> GetByGuidAsync(Guid id)
            {
                return await Table.FindAsync(id);
            }

            public async Task<T> UpdateAsync(T entity)
            {
                await Task.Run(() => Table.Update(entity));
                return entity;
            }

            public async Task<bool> DeleteAsync(T entity)
            {
                try
                {
                    await Task.Run(() => Table.Remove(entity));
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;

                }

            }

            public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            {
                return await Table.AnyAsync(predicate);
            }

            public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
            {
                if (predicate is not null)
                    return await Table.CountAsync(predicate);
                return await Table.CountAsync();
            }


        }
}

