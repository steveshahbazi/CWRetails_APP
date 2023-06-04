using System.ComponentModel.DataAnnotations;

namespace CWRetails_Web.Models.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? PizzeriaName { get; set; }
        public List<PizzaDto>? Pizzas { get; set; }
    }
}
