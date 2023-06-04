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

            IQueryable<Pizza> menuQuery = _db.Pizzas.Where(p => p.PizzeriaId == pizzeria.Id).Distinct();
            menuQuery = menuQuery.Include(pizza => pizza.Ingredients);
            List<Pizza> pizzas = await menuQuery.ToListAsync();
            List<PizzaDto> menu = _mapper.Map<List<PizzaDto>>(pizzas);
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
            if(!isSubset)
                return BadRequest();

            decimal totalPrice = order.Pizzas.Sum(p =>
            {
                int pizzaCount = p.PizzaCount;
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

            // Retrieve the pizzeria from the database based on the name
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);

            if (pizzeria == null)
                return NotFound("Pizzeria not found");

            // Retrieve the pizza from the pizzeria's menu based on the name
            var pizza = pizzeria.Pizzas.FirstOrDefault(p => p.Name == updatedPizza.Name);

            if (pizza == null)
                return NotFound("Pizza not found");

            // Update the pizza's properties with the values from the updatedPizza DTO
            pizza.Name = updatedPizza.Name;
            pizza.BasePrice = updatedPizza.BasePrice;

            // Clear the existing ingredients
            pizza.Ingredients.Clear();

            // Add the updated ingredients to the pizza
            foreach (var ingredientDto in updatedPizza.Ingredients)
            {
                var ingredient = await _db.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Name = ingredientDto.Name
                    };
                }

                pizza.Ingredients.Add(ingredient);
            }

            _db.SaveChanges();

            return Ok("Pizza updated successfully");
        }

        //[HttpPost("addPizzeria")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> AddPizzeria(PizzeriaDto newPizzeria)
        //{
        //    if (newPizzeria == null)
        //        return BadRequest();

        //    if (_db.Pizzerias.Any(p => p.Name == newPizzeria.Name))
        //        return Conflict("A pizzeria with the same name already exists.");

        //    var pizzeriaEntity = new Pizzeria
        //    {
        //        Name = newPizzeria.Name,
        //    };

        //    foreach (var newPizza in newPizzeria.Pizzas)
        //    {
        //        var pizzaEntity = _mapper.Map<Pizza>(newPizza);

        //        foreach (var ingredientDto in newPizza.Ingredients)
        //        {
        //            var ingredientEntity = await _db.Ingredients.AsNoTracking().FirstOrDefaultAsync(i => i.Id == ingredientDto.Id);
        //            if (ingredientEntity != null)
        //            {
        //                var ingredientPizza = new IngredientPizza
        //                {
        //                    Pizza = pizzaEntity,
        //                    Ingredient = ingredientEntity
        //                };
        //                pizzaEntity.IngredientPizza.Add(ingredientPizza);
        //            }
        //            await _db.SaveChangesAsync();
        //        }

        //        pizzeriaEntity.Pizzas.Add(pizzaEntity);
        //    }

        //    _db.Pizzerias.Add(pizzeriaEntity);
        //    await _db.SaveChangesAsync();

        //    return Created("", pizzeriaEntity);
        //}


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


        //[HttpPost("updatePizzeria")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> UpdatePizzeria(string pizzeriaName, PizzeriaDto updatedPizzeria)
        //{
        //    if (updatedPizzeria == null)
        //        return BadRequest();

        //    var pizzeriaEntity = await _db.Pizzerias.Include(p => p.Pizzas)
        //                                            .FirstOrDefaultAsync(p => p.Name == pizzeriaName);

        //    if (pizzeriaEntity == null)
        //        return NotFound();

        //    if (_db.Pizzerias.Any(p => p.Id != updatedPizzeria.Id && p.Name == updatedPizzeria.Name))
        //        return Conflict("A pizzeria with the same name already exists.");

        //    _mapper.Map(updatedPizzeria, pizzeriaEntity);

        //    await _db.SaveChangesAsync();

        //    return Ok();
        //}


        //[HttpPut("updatePizzeria")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> UpdatePizzeria(PizzeriaDto updatedPizzeria)
        //{
        //    if (updatedPizzeria == null)
        //        return BadRequest();

        //    var pizzeriaEntity = await _db.Pizzerias.Include(p => p.Pizzas).ThenInclude(pizza => pizza.IngredientPizza)
        //                                            .FirstOrDefaultAsync(p => p.Id == updatedPizzeria.Id);

        //    if (pizzeriaEntity == null)
        //        return NotFound();

        //    if (_db.Pizzerias.Any(p => p.Id != updatedPizzeria.Id && p.Name == updatedPizzeria.Name))
        //        return Conflict("A pizzeria with the same name already exists.");

        //    // Update the pizzeria name if provided
        //    if (!string.IsNullOrWhiteSpace(updatedPizzeria.Name))
        //        pizzeriaEntity.Name = updatedPizzeria.Name;

        //    // Update the pizzas and their ingredients
        //    foreach (var updatedPizza in updatedPizzeria.Pizzas)
        //    {
        //        var pizzaEntity = pizzeriaEntity.Pizzas.FirstOrDefault(p => p.Name == updatedPizza.Name);

        //        if (pizzaEntity != null)
        //        {
        //            // Update the pizza name if provided
        //            if (!string.IsNullOrWhiteSpace(updatedPizza.Name))
        //                pizzaEntity.Name = updatedPizza.Name;

        //            // Update the pizza's ingredients
        //            var existingIngredientIds = pizzaEntity.IngredientPizza.Select(ip => ip.IngredientId).ToList();
        //            var updatedIngredientIds = updatedPizza.Ingredients.Select(i => i.Id).ToList();

        //            // Remove ingredients that are not in the updated list
        //            var ingredientsToRemove = pizzaEntity.IngredientPizza.Where(ip => !updatedIngredientIds.Contains(ip.IngredientId)).ToList();
        //            foreach (var ingredientToRemove in ingredientsToRemove)
        //                pizzaEntity.IngredientPizza.Remove(ingredientToRemove);

        //            // Add new ingredients
        //            var ingredientsToAdd = updatedPizza.Ingredients.Where(i => !existingIngredientIds.Contains(i.Id)).ToList();
        //            foreach (var ingredientToAdd in ingredientsToAdd)
        //            {
        //                var ingredientEntity = await _db.Ingredients.FindAsync(ingredientToAdd.Id);
        //                if (ingredientEntity != null)
        //                {
        //                    var ingredientPizza = new IngredientPizza
        //                    {
        //                        Pizza = pizzaEntity,
        //                        Ingredient = ingredientEntity
        //                    };
        //                    pizzaEntity.IngredientPizza.Add(ingredientPizza);
        //                }
        //            }
        //        }
        //    }

        //    await _db.SaveChangesAsync();

        //    return NoContent();
        //}


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

        //[HttpDelete("deletePizzeria")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> DeletePizzeria(string pizzeriaName)
        //{
        //    var pizzeriaEntity = await _db.Pizzerias
        //                                  .Include(p => p.Pizzas)
        //                                  .ThenInclude(pizza => pizza.IngredientPizza)
        //                                  .FirstOrDefaultAsync(p => p.Name == pizzeriaName);

        //    if (pizzeriaEntity == null)
        //        return NotFound();

        //    _db.IngredientPizza.RemoveRange(pizzeriaEntity.Pizzas.SelectMany(pizza => pizza.IngredientPizza));
        //    _db.Pizzas.RemoveRange(pizzeriaEntity.Pizzas);
        //    _db.Pizzerias.Remove(pizzeriaEntity);

        //    await _db.SaveChangesAsync();

        //    return Ok();
        //}

        [HttpPost("addPizzaToMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPizzaToMenu(string pizzeriaName, PizzaDto pizza)
        {
            if (pizza == null || string.IsNullOrEmpty(pizzeriaName))
                return BadRequest(); 
            
            var pizzeriaEntity = await _db.Pizzerias.Include(p => p.Pizzas)
                                                    .FirstOrDefaultAsync(p => p.Name == pizzeriaName);

            if (pizzeriaEntity == null)
                return NotFound();

            var pizzaEntity = _mapper.Map<Pizza>(pizza);
            pizzeriaEntity.Pizzas.Add(pizzaEntity);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("updatePizzaInMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePizzaInMenu(string pizzeriaName, int pizzaId, PizzaDto updatedPizza)
        {
            if (updatedPizza == null || string.IsNullOrEmpty(pizzeriaName) || pizzaId <= 0)
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var pizza = pizzeria.Pizzas.FirstOrDefault(p => p.Id == pizzaId);
            if (pizza == null)
                return NotFound();

            if (pizzeria.Pizzas.Any(p => p.Id != pizzaId && p.Name == updatedPizza.Name))
                return Conflict("A pizza with the same name already exists in the menu.");

            pizza = DtoToDatabase.DtoToPizza(updatedPizza);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("deletePizzaFromMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePizzaFromMenu(string pizzeriaName, int pizzaId)
        {
            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var pizza = pizzeria.Pizzas.FirstOrDefault(p => p.Id == pizzaId);
            if (pizza == null)
                return NotFound();

            pizzeria.Pizzas.Remove(pizza);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
