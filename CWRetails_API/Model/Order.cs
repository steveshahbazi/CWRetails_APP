namespace CWRetails_API.Model
{
    public class Order
    {
        public int Id { get; set; }
        public string PizzeriaName { get; set; }
        public List<string> PizzaNames { get; set; }
        public int ToppingsCount { get; set; }
    }
}
