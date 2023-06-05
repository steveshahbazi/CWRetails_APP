using CWRetails_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using CWRetails_API.Repository.IRepository;
using CWRetails_API.Model;

namespace CWRetails_API.Repository
{
    public class PizzeriaRepository : IPizzeriaRepository
    {
        private readonly ApplicationDbContext _db;
        public PizzeriaRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task CreateAsync(Pizzeria pizzeria)
        {
            await _db.Pizzerias.AddAsync(pizzeria);
            await SaveAsync();
        }

        public async Task<Pizzeria> GetAsync(Expression<Func<Pizzeria, bool>>? filter, bool tracked = true)
        {
            IQueryable<Pizzeria> query = _db.Pizzerias;//this should be fixed in the new version to optimise.Include(p => p.Pizzas).ThenInclude(pizza => pizza.Ingredients);
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Pizzeria>> GetAllAsync(Expression<Func<Pizzeria, bool>>? filter)
        {
            IQueryable<Pizzeria> query = _db.Pizzerias;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(Pizzeria pizzeria)
        {
            _db.Pizzerias.Remove(pizzeria);
            await SaveAsync();
        }

        public async Task UpdateAsync(Pizzeria pizzeria)
        {
            _db.Pizzerias.Update(pizzeria);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
