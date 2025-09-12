namespace QLVPP.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IUnitRepository Unit { get; }
        Task<int> SaveChanges();
    }
}
