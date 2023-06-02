using CWRetails_API.Data;
using CWRetails_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CWRetails_API.Data.Pizzerias;

namespace CWRetails_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        [HttpGet("pizzerias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPizzeriaNames()
        {
            var pizzeriaNames = pizzerias.Select(p => p.Name).ToList();
            return Ok(pizzeriaNames);
        }

        [HttpGet("menu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMenu(string pizzeriaName)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            return Ok(pizzeria.Menu);
        }

        [HttpPost("calculateTotalPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CalculateTotalPrice(Order order)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == order.PizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var pizzas = pizzeria.Menu.Where(p => order.PizzaNames.Contains(p.Name)).ToList();
            decimal totalPrice = pizzas.Sum(p => p.TotalPrice);

            return Ok(totalPrice);
        }

        [HttpPost("addPizzeria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AddPizzeria(Pizzeria pizzeria)
        {
            if(pizzeria == null)
                return BadRequest();

            if (pizzerias.Any(p => p.Name == pizzeria.Name))
                return Conflict("A pizzeria with the same name already exists.");

            pizzeria.Id = GetNextPizzeriaId();
            pizzerias.Add(pizzeria);
            return CreatedAtAction(nameof(GetPizzariaById), new { id = pizzeria.Id }, pizzeria);
        }

        [HttpGet("pizzaria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPizzariaById(int pizzariaId)
        {
            if (pizzariaId <= 0)
                return BadRequest();

            var pizzeria = pizzerias.FirstOrDefault(p => p.Id == pizzariaId);
            if (pizzeria == null)
                return NotFound();

            return Ok(pizzeria);
        }

        [HttpPut("updatePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult UpdatePizzeria(string pizzeriaName, Pizzeria updatedPizzeria)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            if (pizzerias.Any(p => p.Name != pizzeriaName && p.Name == updatedPizzeria.Name))
                return Conflict("A pizzeria with the same name already exists.");

            pizzeria.Name = updatedPizzeria.Name;
            pizzeria.Menu = updatedPizzeria.Menu;

            return Ok();
        }

        [HttpDelete("deletePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePizzeria(string pizzeriaName)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            pizzerias.Remove(pizzeria);
            return Ok();
        }

        [HttpPost("addPizzaToMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddPizzaToMenu(string pizzeriaName, Pizza pizza)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            pizza.Id = GetNextPizzaId();
            pizzeria.Menu.Add(pizza);

            return Ok();
        }

        [HttpPut("updatePizzaInMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult UpdatePizzaInMenu(string pizzeriaName, int pizzaId, Pizza updatedPizza)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var pizza = pizzeria.Menu.FirstOrDefault(p => p.Id == pizzaId);
            if (pizza == null)
                return NotFound();

            if (pizzeria.Menu.Any(p => p.Id != pizzaId && p.Name == updatedPizza.Name))
                return Conflict("A pizza with the same name already exists in the menu.");

            pizza.Name = updatedPizza.Name;
            pizza.Toppings = updatedPizza.Toppings;
            pizza.BasePrice = updatedPizza.BasePrice;

            return Ok();
        }

        [HttpDelete("deletePizzaFromMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePizzaFromMenu(string pizzeriaName, int pizzaId)
        {
            var pizzeria = pizzerias.FirstOrDefault(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            var pizza = pizzeria.Menu.FirstOrDefault(p => p.Id == pizzaId);
            if (pizza == null)
                return NotFound();

            pizzeria.Menu.Remove(pizza);
            return Ok();
        }

        private static int GetNextPizzaId()
        {
            int maxId = pizzerias.SelectMany(p => p.Menu).Max(pizza => pizza.Id);
            return maxId + 1;
        }
        
        private static int GetNextPizzeriaId()
        {
            int maxId = pizzerias.Max(p => p.Id);
            return maxId + 1;
        }
    }
}
