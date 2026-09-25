namespace lab_01.Repositories.Implementations
{
    using global::lab_01.Data;
    using global::lab_01.Models.Entities.Base;
    using global::lab_01.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    namespace lab_01.Repositories.Implementations
    {
        public class Repository<T> : IRepository<T>
            where T : BaseEntry
        {
            protected readonly AppDbContext _context;
            protected readonly DbSet<T> _dbSet;

            public Repository(AppDbContext context)
            {
                _context = context;
                _dbSet = context.Set<T>();
            }

            public async Task<T?> GetByIdAsync(string id)
            {
                return await _dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(entity => entity.Id == id);
            }

            public async Task<IReadOnlyList<T>> GetAllAsync()
            {
                return await _dbSet
                    .AsNoTracking()
                    .ToListAsync();
            }

            public async Task AddAsync(T entity)
            {
                await _dbSet.AddAsync(entity);
            }

            public void Update(T entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;

                _dbSet.Update(entity);
            }

            public void Delete(T entity)
            {
                _dbSet.Remove(entity);
            }

            public async Task<int> SaveChangesAsync()
            {
                return await _context.SaveChangesAsync();
            }
        }
    }
}
