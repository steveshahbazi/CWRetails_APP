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

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Pizzeria> Pizzerias { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<IngredientPizza> IngredientPizza { get; set; }
        public DbSet<ToppingPizza> ToppingPizza { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizzeria>()
                .HasMany(e => e.Pizzas)
                .WithOne(e => e.Pizzeria)
                .HasForeignKey(e => e.PizzeriaId)
                .IsRequired(false);

            modelBuilder.Entity<Pizzeria>()
                .HasMany(e => e.Orders)
                .WithOne(e => e.Pizzeria)
                .HasForeignKey(e => e.PizzeriaId)
                .IsRequired();//an order shuold have a pizzeria

            modelBuilder.Entity<Pizza>()
                .HasMany(e => e.Ingredients)
                .WithMany(e => e.Pizzas)
                .UsingEntity<IngredientPizza>();

            modelBuilder.Entity<Pizza>()
                .HasMany(p => p.Toppings)
                .WithMany(e => e.Pizzas)
                .UsingEntity<ToppingPizza>();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Pizzas)
                .WithMany(e => e.Orders);

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

            modelBuilder.Entity<Topping>().HasData(
                new Topping { Id = 1, Name = "Cheese", Price = 1 },
                new Topping { Id = 2, Name = "Capsicum", Price = 1 },
                new Topping { Id = 3, Name = "Salami", Price = 1 },
                new Topping { Id = 4, Name = "Olives", Price = 1 }
            );

            modelBuilder.Entity<Pizzeria>().HasData(
                new Pizzeria { Id = 1, Name = "Preston Pizzeria" },
                new Pizzeria { Id = 2, Name = "Southbank Pizzeria" }
            );

            modelBuilder.Entity<Pizza>().HasData(
                new Pizza { Id = 1, Name = "Capricciosa", BasePrice = 20, PizzeriaId = 1,  },
                new Pizza { Id = 2, Name = "Mexicana", BasePrice = 18, PizzeriaId = 1 },
                new Pizza { Id = 3, Name = "Margherita", BasePrice = 22, PizzeriaId = 1 },
                new Pizza { Id = 4, Name = "Capricciosa", BasePrice = 25, PizzeriaId = 2 },
                new Pizza { Id = 5, Name = "Vegetarian", BasePrice = 17, PizzeriaId = 2 }
            );

            modelBuilder.Entity<IngredientPizza>().HasData(
                // Capricciosa Ingredients
                new { PizzaId = 1, IngredientId = 1 }, // Cheese
                new { PizzaId = 1, IngredientId = 2 }, // Ham
                new { PizzaId = 1, IngredientId = 3 }, // Mushrooms
                new { PizzaId = 1, IngredientId = 4 }, // Olives

                // Mexicana Ingredients
                new { PizzaId = 2, IngredientId = 1 }, // Cheese
                new { PizzaId = 2, IngredientId = 5 }, // Salami
                new { PizzaId = 2, IngredientId = 6 }, // Capsicum
                new { PizzaId = 2, IngredientId = 7 }, // Chilli

                // Margherita Ingredients
                new { PizzaId = 3, IngredientId = 1 },  // Cheese
                new { PizzaId = 3, IngredientId = 8 },  // Spinach
                new { PizzaId = 3, IngredientId = 9 },  // Ricotta
                new { PizzaId = 3, IngredientId = 10 }, // Cherry Tomatoes

                // Capricciosa Ingredients (Second instance)
                new { PizzaId = 4, IngredientId = 1 }, // Cheese
                new { PizzaId = 4, IngredientId = 2 }, // Ham
                new { PizzaId = 4, IngredientId = 3 }, // Mushrooms
                new { PizzaId = 4, IngredientId = 4 }, // Olives

                // Vegetarian Ingredients
                new { PizzaId = 5, IngredientId = 1 },  // Cheese
                new { PizzaId = 5, IngredientId = 3 },  // Mushrooms
                new { PizzaId = 5, IngredientId = 6 },  // Capsicum
                new { PizzaId = 5, IngredientId = 11 }, // Onion
                new { PizzaId = 5, IngredientId = 4 }   // Olives
            );
        }
    }
}
