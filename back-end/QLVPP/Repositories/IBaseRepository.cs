namespace QLVPP.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T?> GetById(object id);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
