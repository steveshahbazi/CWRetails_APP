using System.ComponentModel.DataAnnotations;

namespace CWRetails_Web.Models.DTO
{
    public class IngredientDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
