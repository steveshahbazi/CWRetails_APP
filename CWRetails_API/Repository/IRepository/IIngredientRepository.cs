using CWRetails_API.Model;
using System.Linq.Expressions;

namespace CWRetails_API.Repository.IRepository
{
    public interface IIngredientRepository
    {
        Task<List<Ingredient>> GetAllAsync(Expression<Func<Ingredient, bool>>? filter = null);
        Task<Ingredient> GetAsync(Expression<Func<Ingredient, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(Ingredient pizza);
        Task RemoveAsync(Ingredient pizza);
        Task UpdateAsync(Ingredient pizza);
        Task SaveAsync();
    }
}
