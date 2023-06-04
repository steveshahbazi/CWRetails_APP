using AutoMapper;
using CWRetails_API.Data;
using CWRetails_API.Model;
using CWRetails_API.Model.Converter;
using CWRetails_API.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CWRetails_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public PizzaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("pizzerias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPizzeriaNames()
        {
            var pizzeriaNames = await _db.Pizzerias.Select(p => p.Name).ToListAsync();
            return Ok(pizzeriaNames);
        }

        [HttpGet("menu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMenu(string pizzeriaName)
        {
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var menuQuery = _db.Pizzas.Include(p => p.Ingredients)
                                      .Where(p => p.PizzeriaId == pizzeria.Id)
                                      .Distinct();

            var pizzas = await menuQuery.ToListAsync();
            var menu = _mapper.Map<List<PizzaDto>>(pizzas);
            return Ok(menu);
        }


        [HttpPost("calculateTotalPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalculateTotalPrice(OrderDto order)
        {
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == order.PizzeriaName);
            if (pizzeria == null)
                return NotFound();

            if (order == null || order.Pizzas == null)
                return BadRequest();

            IQueryable<Pizza> menuQuery = _db.Pizzas.Where(p => p.PizzeriaId == pizzeria.Id).Distinct();
            menuQuery = menuQuery.Include(pizza => pizza.Ingredients);
            List<Pizza> menu = await menuQuery.ToListAsync();

            bool isSubset = order.Pizzas.All(p => menu.Any(item => item.Name == p.Name));
            if (!isSubset)
                return BadRequest();

            decimal totalPrice = order.Pizzas.Sum(p =>
            {
                var matchingPizza = menu.FirstOrDefault(pizza => pizza.Id == p.Id);
                if (matchingPizza != null)
                {
                    p.BasePrice = matchingPizza.BasePrice;
                    return p.TotalPrice;
                }
                return 0;
            });

            return Ok(totalPrice);
        }

        [HttpPut("updatePizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePizza(string pizzeriaName, PizzaDto updatedPizza)
        {
            if (updatedPizza == null)
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound("Pizzeria not found");

            var pizza = await _db.Pizzas.Include(p => p.Ingredients)
                                      .Where(p => p.PizzeriaId == updatedPizza.Id && p.Id == updatedPizza.Id)
                                      .FirstOrDefaultAsync();

            if (pizza == null)
                return NotFound("Pizza not found");

            // Update the pizza's properties with the values from the updatedPizza DTO
            pizza.Name = updatedPizza.Name;
            pizza.BasePrice = updatedPizza.BasePrice;

            // Clear the existing ingredients except for the ones present in the updatedPizza
            pizza.Ingredients.RemoveAll(i => !updatedPizza.Ingredients.Any(ingredientDto => ingredientDto.Id == i.Id));

            // Add the updated ingredients to the pizza
            foreach (var ingredientDto in updatedPizza.Ingredients)
            {
                // Check if the ingredient already exists in the pizza's ingredients
                var existingIngredient = pizza.Ingredients.FirstOrDefault(i => i.Id == ingredientDto.Id);

                if (existingIngredient == null)
                {
                    // Retrieve the ingredient from the database or create a new one
                    var ingredient = await _db.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredientDto.Id);

                    if (ingredient == null)
                    {
                        ingredient = new Ingredient
                        {
                            Id = ingredientDto.Id,
                            Name = ingredientDto.Name
                        };
                    }

                    // Add the ingredient to the pizza
                    pizza.Ingredients.Add(ingredient);
                }
                else
                {
                    // Update the existing ingredient's name if it has changed
                    if (existingIngredient.Name != ingredientDto.Name)
                    {
                        existingIngredient.Name = ingredientDto.Name;
                    }
                }
            }

            _db.SaveChanges();

            return Ok("Pizza updated successfully");
        }

        [HttpPost("addPizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPizza(string pizzeriaName, PizzaDto newPizza)
        {
            if (newPizza == null)
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound("Pizzeria not found");

            var pizza = new  Pizza
            {
                Name = newPizza.Name,
                BasePrice = newPizza.BasePrice,
                Pizzeria = pizzeria,
                Ingredients = new List<Ingredient>()
            };

            foreach (var ingredientDto in newPizza.Ingredients)
            {
                var ingredient = await _db.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredientDto.Id);
                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Id = ingredientDto.Id,
                        Name = ingredientDto.Name
                    };
                }

                pizza.Ingredients.Add(ingredient);
            }

            _db.Pizzas.Add(pizza);
            await _db.SaveChangesAsync();

            var pizzaDto = _mapper.Map<Pizza, PizzaDto>(pizza);
            return Ok(pizzaDto);
        }

        [HttpGet("getPizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPizza(string pizzeriaName, int pizzaId)
        {
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound("Pizzeria not found");

            var pizza = await _db.Pizzas.Include(p => p.Ingredients)
                                        .FirstOrDefaultAsync(p => p.PizzeriaId == pizzeria.Id && p.Id == pizzaId);
            if (pizza == null)
                return NotFound("Pizza not found");

            var pizzaDto = _mapper.Map<Pizza, PizzaDto>(pizza);

            return Ok(pizzaDto);
        }


        [HttpPost("addPizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPizzeria(PizzeriaDto newPizzeria)
        {
            if (newPizzeria == null)
                return BadRequest();

            var pizzeria = new Pizzeria
            {
                Name = newPizzeria.Name
            };

            _db.Pizzerias.Add(pizzeria);
            await _db.SaveChangesAsync();

            foreach (var newPizza in newPizzeria.Pizzas)
            {
                await AddPizza(newPizzeria.Name, newPizza);
            }

            return Ok("Pizzeria is added successfully!");
        }

        [HttpGet("{pizzeriaId}", Name = "GetPizzeriaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPizzeriaById(int pizzeriaId)
        {
            var pizzeriaEntity = await _db.Pizzerias.Include(p => p.Pizzas).ThenInclude(pizza => pizza.Ingredients)
                                                  .FirstOrDefaultAsync(p => p.Id == pizzeriaId);

            if (pizzeriaEntity == null)
                return NotFound();

            var pizzeriaDto = _mapper.Map<PizzeriaDto>(pizzeriaEntity);
            return Ok(pizzeriaDto);
        }

        [HttpDelete("deletePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePizzeria(string pizzeriaName)
        {
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            _db.Pizzerias.Remove(pizzeria);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
