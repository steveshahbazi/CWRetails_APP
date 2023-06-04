using CWRetails_API.Model.DTO;

namespace CWRetails_API.Model.Converter
{
    public class DtoToDatabase
    {
        public static Ingredient DtoToIngredient(IngredientDto dto)
        {
            Ingredient ingredient = new()
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
            };
            return ingredient;
        }

        public static Pizza DtoToPizza(PizzaDto dto)
        {
            Pizza pizza = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                BasePrice = dto.BasePrice,
                Ingredients = dto.Ingredients.Select(i => DtoToIngredient(i)).ToList(),
                //Toppings = dto.Toppings.Select((t => DtoToIngredient(t))).ToList()
            };
            return pizza;
        }

        public static Pizzeria DtoToPizzeria(PizzeriaDto dto)
        {
            Pizzeria pizzeria = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Pizzas = dto.Pizzas.Select(p => DtoToPizza(p)).ToList(),
            };
            return pizzeria;
        }
    }
}
