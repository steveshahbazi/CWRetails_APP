﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CWRetails_API.Model.DTO
{
    public class PizzaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
        public ICollection<ToppingDto> Toppings { get; set; } = new List<ToppingDto>();
        public decimal BasePrice { get; set; }
        public int PizzaCount { get; set; }
        public decimal TotalPrice => PizzaCount * (BasePrice + (Toppings?.Sum(t => t.Price) ?? 0));
    }
}