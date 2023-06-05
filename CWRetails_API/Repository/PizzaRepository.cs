using CWRetails_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using CWRetails_API.Repository.IRepository;
using CWRetails_API.Model;

namespace CWRetails_API.Repository
{
    public class PizzaRepository : IPizzaRepository
    {
        private readonly ApplicationDbContext _db;
        public PizzaRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task CreateAsync(Pizza pizza)
        {
            await _db.Pizzas.AddAsync(pizza);
            await SaveAsync();
        }

        public async Task<Pizza> GetAsync(Expression<Func<Pizza, bool>>? filter, bool tracked = true)
        {
            IQueryable<Pizza> query = _db.Pizzas.Include(p => p.Ingredients);
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

        public async Task<List<Pizza>> GetAllAsync(Expression<Func<Pizza, bool>>? filter)
        {
            IQueryable<Pizza> query = _db.Pizzas.Include(p => p.Ingredients);
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.Distinct().ToListAsync();
        }

        public async Task RemoveAsync(Pizza pizza)
        {
            _db.Pizzas.Remove(pizza);
            await SaveAsync();
        }

        public async Task UpdateAsync(Pizza pizza)
        {
            _db.Pizzas.Update(pizza);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
