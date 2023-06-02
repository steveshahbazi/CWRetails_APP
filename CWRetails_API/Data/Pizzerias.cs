using CWRetails_API.Model;

namespace CWRetails_API.Data
{
    public static class Pizzerias
    {
        public static List<Pizzeria> pizzerias = new()
        {
            new Pizzeria
            {
                Name = "Preston Pizzeria",
                Menu = new List<Pizza>
                {
                    new Pizza
                    {
                        Id = 1,
                        Name = "Capricciosa",
                        Toppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 2, Name = "Ham", Price = 1 },
                            new Topping { Id = 3, Name = "Mushrooms", Price = 1 },
                            new Topping { Id = 4, Name = "Olives", Price = 1 }
                        },
                        ExtraToppings = new List<Topping>
                        {
                            new Topping { Id = 5, Name = "Salami", Price = 1 },
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 6, Name = "Capsicum", Price = 1 },
                            new Topping { Id = 4, Name = "Olives", Price = 1 }
                        },
                        BasePrice = 20
                    },
                    new Pizza
                    {
                        Id = 2,
                        Name = "Mexicana",
                        Toppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 5, Name = "Salami", Price = 1 },
                            new Topping { Id = 6, Name = "Capsicum", Price = 1 },
                            new Topping { Id = 7, Name = "Chilli", Price = 1 }
                        },
                        ExtraToppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 6, Name = "Capsicum", Price = 1 },
                        },
                        BasePrice = 18
                    },
                    new Pizza
                    {
                        Id = 3,
                        Name = "Margherita",
                        Toppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 8, Name = "Spinach", Price = 1 },
                            new Topping { Id = 9, Name = "Ricotta", Price = 1 },
                            new Topping { Id = 10, Name = "Cherry Tomatoes", Price = 1 }
                        },
                        ExtraToppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                        },
                        BasePrice = 22
                    }
                }
            },
            new Pizzeria
            {
                Name = "Southbank Pizzeria",
                Menu = new List<Pizza>
                {
                    new Pizza
                    {
                        Id = 4,
                        Name = "Capricciosa",
                        Toppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 2, Name = "Ham", Price = 1 },
                            new Topping { Id = 3, Name = "Mushrooms", Price = 1 },
                            new Topping { Id = 4, Name = "Olives", Price = 1 }
                        },
                        ExtraToppings = new List<Topping>
                        {
                            new Topping { Id = 5, Name = "Salami", Price = 1 },
                        },
                        BasePrice = 25
                    },
                    new Pizza
                    {
                        Id = 5,
                        Name = "Vegetarian",
                        Toppings = new List<Topping>
                        {
                            new Topping { Id = 1, Name = "Cheese", Price = 1 },
                            new Topping { Id = 3, Name = "Mushrooms", Price = 1 },
                            new Topping { Id = 6, Name = "Capsicum", Price = 1 },
                            new Topping { Id = 11, Name = "Onion", Price = 1 },
                            new Topping { Id = 4, Name = "Olives", Price = 1 }
                        },
                        BasePrice = 17
                    }
                }
            }
        };
    }
}
