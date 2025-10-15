namespace QLVPP.Repositories
{
    public interface IBaseRepository<T>
        where T : class
    {
        Task<List<T>> GetAll();
        Task<T?> GetById(object id);
        Task Add(T entity);
        Task Update(T entity);
    }
}
