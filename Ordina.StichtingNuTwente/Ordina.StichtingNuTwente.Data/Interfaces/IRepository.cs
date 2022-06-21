using System.Linq.Expressions;

namespace Ordina.StichtingNuTwente.Data
{
    public interface IRepository<T> where T : class, new()
    {
        public int Count();
        public T Create(T entity);
        public void Delete(T entity);
        public T Get(Expression<Func<T, bool>> where, string includeProperties = null);
        public IEnumerable<T> GetAll(string includeProperties = null);
        public T GetById(int id, string includeProperties = null);
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        public IEnumerable<T> GetMany(Expression<Func<T, bool>> where, string includeProperties = null);
        public void Update(T entity);
    }
}