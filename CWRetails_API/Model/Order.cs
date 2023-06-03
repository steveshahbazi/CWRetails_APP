using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PizzeriaId { get; set; }
        public Pizzeria Pizzeria { get; set; } = new Pizzeria();

        public ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
    }
}
