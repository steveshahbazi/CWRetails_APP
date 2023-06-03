using CWRetails_API.Model.DTO;

namespace CWRetails_API.Model.Converter
{
    public class DatabaseToDto
    {
        public static IngredientDto IngredientToDto(Ingredient ingredient)
        {
            IngredientDto dto = new()
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
            };
            return dto;
        }

        public static OrderDto OrderToDto(Order order)
        {
            OrderDto dto = new()
            {
                Id = order.Id,
                Pizzas = order.Pizzas.Select(p => PizzaToDto(p)).ToList(),
                PizzeriaName = order.Pizzeria.Name
            };
            return dto;
        }

        public static PizzaDto PizzaToDto(Pizza pizza)
        {
            PizzaDto dto = new()
            {
                Id = pizza.Id,
                Name = pizza.Name,
                BasePrice = pizza.BasePrice,
                Ingredients = pizza.Ingredients.Select(i => IngredientToDto(i)).ToList(),
                //Toppings = pizza.Toppings.Select((t => IngredientToDto(t))).ToList()
            };
            return dto;
        }

        public static PizzeriaDto PizzeriaToDto(Pizzeria pizzeria)
        {
            PizzeriaDto dto = new()
            {
                Id = pizzeria.Id,
                Name = pizzeria.Name,
                Pizzas = pizzeria.Pizzas.Select(m => PizzaToDto(m)).ToList(),
            };
            return dto;
        }
    }
}
