namespace PrototipoApi.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using PrototipoApi.BaseDatos;
    using PrototipoApi.Repositories.Interfaces;
    using System.Linq.Expressions;


    // Repositorio genérico para operaciones CRUD y consultas avanzadas sobre cualquier entidad
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ContextoBaseDatos _context;
        private readonly DbSet<T> _dbSet;

        // Constructor: recibe el contexto de base de datos e inicializa el DbSet
        public Repository(ContextoBaseDatos context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Obtiene una entidad que cumpla el predicado, permitiendo incluir relaciones
        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate,
                                  params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        // Obtiene todas las entidades, permite ordenar e incluir relaciones
        public async Task<List<T>> GetAllAsync(
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes
        )
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        // Proyección de una entidad a un DTO, filtrando y seleccionando campos específicos
        public async Task<TResult?> SelectOneAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            CancellationToken ct = default,
            bool asNoTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking) query = query.AsNoTracking();

            return await query
                .Where(filter)
                .Select(selector)
                .FirstOrDefaultAsync(ct);
        }

        // Proyección de una lista de entidades a DTOs, con filtros, orden, paginación y selección de campos
        public async Task<List<TResult>> SelectListAsync<TResult>(
            Expression<Func<T, bool>>? filter,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
            Expression<Func<T, TResult>> selector,
            int? skip = null,
            int? take = null,
            CancellationToken ct = default,
            bool asNoTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking) query = query.AsNoTracking();
            if (filter != null) query = query.Where(filter);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.Select(selector).ToListAsync(ct);
        }

        // Busca una entidad por su clave primaria (ID)
        public async Task<T?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        // Agrega una nueva entidad al contexto
        public async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);

        // Marca una entidad como modificada
        public async Task UpdateAsync(T entity, Action? extraAction = null)
        {
            _dbSet.Update(entity);
            extraAction?.Invoke();
            await Task.CompletedTask;
        }

        // Elimina una entidad del contexto
        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        // Verifica si existe una entidad con el ID dado
        public async Task<bool> ExistsAsync(int id) =>
            await _dbSet.FindAsync(id) != null;

        // Verifica si existe alguna entidad que cumpla el filtro
        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default) => _dbSet.AnyAsync(filter, ct);

        // Cuenta la cantidad de entidades que cumplen el filtro (o todas si es null)
        public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);
            return query.CountAsync(ct);
        }

        // Guarda los cambios pendientes en la base de datos
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Manejo de excepciones de concurrencia
                throw new InvalidOperationException("Error al guardar los cambios en la base de datos.", ex);
            }
            catch (DbUpdateException ex)
            {
                // Manejo de excepciones de actualización
                throw new InvalidOperationException("Error al actualizar la base de datos.", ex);
            }
        }
    }
}
