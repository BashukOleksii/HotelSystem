using lab_01.Models.Entities.Base;

namespace lab_01.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntry
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
