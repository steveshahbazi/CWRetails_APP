namespace CWRetails_API.Model
{
    public class IngredientPizza
    {
        public int PizzaId { get; set; }
        public int IngredientId { get; set; }
        public Pizza Pizza { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}
