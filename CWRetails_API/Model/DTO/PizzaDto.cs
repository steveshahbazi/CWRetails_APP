using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model.DTO
{
    public class PizzaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
        public List<ToppingDto> Toppings { get; set; } = new List<ToppingDto>();
        public decimal BasePrice { get; set; }
        public int PizzaCount { get; set; }

        public decimal TotalPrice => PizzaCount * (BasePrice + GetToppingsPrice());

        private decimal GetToppingsPrice()
        {
            decimal toppingsPrice = 0;
            ICollection<string> toppings = new List<string> { "Cheese", "Capsicum", "Salami", "Olives" };

            if (Toppings != null)
            {
                toppingsPrice = Toppings
                    .Where(ingredient => toppings.Contains(ingredient.Name))
                    .Sum(_ => 1);//each topping costs $1
            }

            return toppingsPrice;
        }
    }
}
