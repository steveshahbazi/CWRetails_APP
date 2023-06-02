namespace CWRetails_API.Model
{
    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Topping>? Toppings { get; set; }
        public List<Topping>? ExtraToppings { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice => BasePrice + (ExtraToppings?.Sum(t => t.Price) ?? 0);
    }
}
