using CWRetails_API.Model;
using System.Linq.Expressions;

namespace CWRetails_API.Repository.IRepository
{
    public interface IPizzeriaRepository
    {
        Task<List<Pizzeria>> GetAllAsync(Expression<Func<Pizzeria, bool>>? filter = null);
        Task<Pizzeria> GetAsync(Expression<Func<Pizzeria, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(Pizzeria pizzeria);
        Task RemoveAsync(Pizzeria pizzeria);
        Task UpdateAsync(Pizzeria pizzeria);
        Task SaveAsync();
    }
}
