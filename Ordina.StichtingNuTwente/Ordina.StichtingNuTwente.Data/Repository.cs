using Microsoft.EntityFrameworkCore;
using Ordina.StichtingNuTwente.Models;

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

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
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

        public T GetById(int id) => entities.SingleOrDefault(x => x.Id == id);

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

    }
}