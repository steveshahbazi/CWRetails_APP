using Azure;
using Microsoft.Extensions.Hosting;

namespace CWRetails_API.Model
{
    public class ToppingPizza
    {
        public int PizzaId { get; set; }
        public int ToppingId { get; set; }
        public Pizza Pizza{ get; set; } = null!;
        public Topping Topping{ get; set; } = null!;
    }
}
