using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? PizzeriaName { get; set; }
        public List<PizzaDto>? Pizzas { get; set; }
        public List<ToppingDto>? Topping { get; set; }
    }
}
