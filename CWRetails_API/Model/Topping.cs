﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model
{
    public class Topping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and numbers.")]
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [Required]
        public decimal Price { get; set; }
        public List<Pizza> Pizzas { get; } = new();
        public List<ToppingPizza> ToppingPizza { get; } = new();

    }
}
