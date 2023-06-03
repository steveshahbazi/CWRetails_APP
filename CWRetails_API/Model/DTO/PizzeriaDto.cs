using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model.DTO
{
    public class PizzeriaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<PizzaDto> Pizzas { get; set; } = new List<PizzaDto>();
    }
}
