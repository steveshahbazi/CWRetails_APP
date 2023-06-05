using CWRetails_API.Data;
using CWRetails_API.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using CWRetails_API.Repository.IRepository;

namespace CWRetails_API.Repository
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _db;
        public IngredientRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task CreateAsync(Ingredient ingredient)
        {
            await _db.Ingredients.AddAsync(ingredient);
            await SaveAsync();
        }

        public async Task RemoveAsync(Ingredient ingredient)
        {
            _db.Ingredients.Remove(ingredient);
            await SaveAsync();
        }

        public async Task UpdateAsync(Ingredient ingredient)
        {
            _db.Ingredients.Update(ingredient);
            await SaveAsync();
        }

        public async Task<List<Ingredient>> GetAllAsync(Expression<Func<Ingredient, bool>>? filter)
        {
            IQueryable<Ingredient> query = _db.Ingredients;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.Distinct().ToListAsync();
        }

        public async Task<Ingredient> GetAsync(Expression<Func<Ingredient, bool>>? filter, bool tracked = true)
        {
            IQueryable<Ingredient> query = _db.Ingredients;
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

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
