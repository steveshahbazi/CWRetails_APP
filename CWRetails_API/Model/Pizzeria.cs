namespace CWRetails_API.Model
{
    public class Pizzeria
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Pizza> Menu { get; set; }
    }
}
