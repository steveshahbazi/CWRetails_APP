using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CWRetails_API.Model
{
    public class Pizza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //optional Pizzeria as we might want to make some pizzas and keep it secret for future stores
        public int? PizzeriaId { get; set; }
        public Pizzeria? Pizzeria { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and number.")]
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<Topping>? Toppings { get; set; }

        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [Required]
        [Precision(18, 2)]
        public decimal BasePrice { get; set; }
        public List<Order> Orders { get; } = new();
        public List<IngredientPizza> IngredientPizza { get; } = new();
        public List<ToppingPizza> ToppingPizza { get; } = new();

        [NotMapped]
        public decimal TotalPrice => BasePrice + GetToppingsPrice();

        private decimal GetToppingsPrice()
        {
            decimal toppingsPrice = 0;
            ICollection<string> toppings = new List<string> { "Cheese", "Capsicum", "Salami", "Olives" };

            if (Toppings != null)
            {
                foreach (var ingredient in Toppings)
                {
                    if (toppings.Contains(ingredient.Name))
                        toppingsPrice += ingredient.Price;
                }
            }

            return toppingsPrice;
        }
    }
}
