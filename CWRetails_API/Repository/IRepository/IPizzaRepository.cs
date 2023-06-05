using CWRetails_API.Model;
using System.Linq.Expressions;

namespace CWRetails_API.Repository.IRepository
{
    public interface IPizzaRepository
    {
        Task<List<Pizza>> GetAllAsync(Expression<Func<Pizza, bool>>? filter = null);
        Task<Pizza> GetAsync(Expression<Func<Pizza, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(Pizza pizza);
        Task RemoveAsync(Pizza pizza);
        Task UpdateAsync(Pizza pizza);
        Task SaveAsync();
    }
}
