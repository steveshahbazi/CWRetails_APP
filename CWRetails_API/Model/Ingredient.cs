using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;

namespace CWRetails_API.Model
{
    public class Ingredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and numbers.")]
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
    }

}
