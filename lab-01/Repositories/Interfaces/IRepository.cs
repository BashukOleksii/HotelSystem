using lab_01.Models.Entities.Base;

namespace lab_01.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntry
    {
        Task<T?> GetByIdAsync(string id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
