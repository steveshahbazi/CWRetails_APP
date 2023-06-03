using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model.DTO
{
    public class IngredientDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
