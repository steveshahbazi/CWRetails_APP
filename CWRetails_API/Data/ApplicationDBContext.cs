using AutoMapper;
using CWRetails_API.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;

namespace CWRetails_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pizzeria> Pizzerias { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizzeria>()
                .HasMany(e => e.Pizzas)
                .WithOne(e => e.Pizzeria)
                .HasForeignKey(e => e.PizzeriaId)
                .IsRequired(false);

            modelBuilder.Entity<Pizza>()
                .HasMany(p => p.Ingredients)
                .WithMany(i => i.Pizzas)
                .UsingEntity(j => j.ToTable("PizzaIngredients"));

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Cheese" },
                new Ingredient { Id = 2, Name = "Ham" },
                new Ingredient { Id = 3, Name = "Mushrooms" },
                new Ingredient { Id = 4, Name = "Olives" },
                new Ingredient { Id = 5, Name = "Salami" },
                new Ingredient { Id = 6, Name = "Capsicum" },
                new Ingredient { Id = 7, Name = "Chilli" },
                new Ingredient { Id = 8, Name = "Spinach" },
                new Ingredient { Id = 9, Name = "Ricotta" },
                new Ingredient { Id = 10, Name = "Cherry Tomatoes" },
                new Ingredient { Id = 11, Name = "Onion" }
            );

            modelBuilder.Entity<Pizzeria>().HasData(
                new Pizzeria { Id = 1, Name = "Preston Pizzeria" },
                new Pizzeria { Id = 2, Name = "Southbank Pizzeria" }
            );

            modelBuilder.Entity<Pizza>().HasData(
                new Pizza { Id = 1, Name = "Capricciosa", BasePrice = 20, PizzeriaId = 1 },
                new Pizza { Id = 2, Name = "Mexicana", BasePrice = 18, PizzeriaId = 1 },
                new Pizza { Id = 3, Name = "Margherita", BasePrice = 22, PizzeriaId = 1 },
                new Pizza { Id = 4, Name = "Capricciosa", BasePrice = 25, PizzeriaId = 2 },
                new Pizza { Id = 5, Name = "Vegetarian", BasePrice = 17, PizzeriaId = 2 }
            );

            modelBuilder.Entity<Pizza>().HasMany(p => p.Ingredients).WithMany(i => i.Pizzas)
                .UsingEntity(j => j.HasData(
                    new { PizzasId = 1, IngredientsId = 1 }, // Capricciosa - Cheese
                    new { PizzasId = 1, IngredientsId = 2 }, // Capricciosa - Ham
                    new { PizzasId = 1, IngredientsId = 3 }, // Capricciosa - Mushrooms
                    new { PizzasId = 1, IngredientsId = 4 }, // Capricciosa - Olives

                    new { PizzasId = 2, IngredientsId = 1 }, // Mexicana - Cheese
                    new { PizzasId = 2, IngredientsId = 5 }, // Mexicana - Salami
                    new { PizzasId = 2, IngredientsId = 6 }, // Mexicana - Capsicum
                    new { PizzasId = 2, IngredientsId = 7 }, // Mexicana - Chilli

                    new { PizzasId = 3, IngredientsId = 1 },  // Margherita - Cheese
                    new { PizzasId = 3, IngredientsId = 8 },  // Margherita - Spinach
                    new { PizzasId = 3, IngredientsId = 9 },  // Margherita - Ricotta
                    new { PizzasId = 3, IngredientsId = 10 }, // Margherita - Cherry Tomatoes

                    new { PizzasId = 4, IngredientsId = 1 }, // Capricciosa (Second instance) - Cheese
                    new { PizzasId = 4, IngredientsId = 2 }, // Capricciosa (Second instance) - Ham
                    new { PizzasId = 4, IngredientsId = 3 }, // Capricciosa (Second instance) - Mushrooms
                    new { PizzasId = 4, IngredientsId = 4 }, // Capricciosa (Second instance) - Olives

                    new { PizzasId = 5, IngredientsId = 1 },  // Vegetarian - Cheese
                    new { PizzasId = 5, IngredientsId = 3 },  // Vegetarian - Mushrooms
                    new { PizzasId = 5, IngredientsId = 6 },  // Vegetarian - Capsicum
                    new { PizzasId = 5, IngredientsId = 11 }, // Vegetarian - Onion
                    new { PizzasId = 5, IngredientsId = 4 }   // Vegetarian - Olives
                ));
        }
    }
}
