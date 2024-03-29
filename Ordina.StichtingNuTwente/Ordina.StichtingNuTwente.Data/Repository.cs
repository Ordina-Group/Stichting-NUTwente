﻿using Microsoft.EntityFrameworkCore;
using Ordina.StichtingNuTwente.Models.Models;
using System.Linq.Expressions;

namespace Ordina.StichtingNuTwente.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private readonly NuTwenteContext context;
        private readonly DbSet<T> entities;
        public string errorMessage = string.Empty;

        public Repository(NuTwenteContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll(string includeProperties = null)
        {
            IQueryable<T> q = entities;
            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    q = q.Include(p);
                }
            }
            return q.AsEnumerable();
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>> where, string includeProperties = null)
        {
            IQueryable<T> q = entities;
            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    q = q.Include(p);
                }
            }

            return q.Where(where).AsEnumerable();
        }

        public T Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            entities.Add(entity);
            context.SaveChanges();

            return entity;
        }

        public T GetById(int id, string includeProperties = null)
        {
            IQueryable<T> q = entities;
            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    q = q.Include(p);
                }
            }
            return q.SingleOrDefault(x => x.Id == id);
        }

        public T Get(Expression<Func<T, bool>> where, string includeProperties = null)
        {
            IQueryable<T> q = entities;
            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    q = q.Include(p);
                }
            }

            return q.SingleOrDefault(where);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> q = entities;
            if (filter != null)
            {
                q = q.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    q = q.Include(p);
                }
            }
            return q.FirstOrDefault();
        }
    

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            entities.Remove(entity);
            context.SaveChanges();
        }

        public int Count()
        {
            return entities.Count();
        }
    }
}