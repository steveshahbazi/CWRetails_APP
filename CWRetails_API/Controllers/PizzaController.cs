using AutoMapper;
using CWRetails_API.Data;
using CWRetails_API.Model;
using CWRetails_API.Model.Converter;
using CWRetails_API.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            //var menu = await _db.Pizzas.Where(p=>p.PizzeriaId == pizzeria.Id).Distinct().ToListAsync();
            //var pizzas = menu.Select(p => DatabaseToDto.PizzaToDto(p)).ToList();

            IQueryable<Pizza> menuQuery = _db.Pizzas.Where(p => p.PizzeriaId == pizzeria.Id).Distinct();
            menuQuery = menuQuery.Include(pizza => pizza.Ingredients);
            List<Pizza> menu = await menuQuery.ToListAsync();
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
                    return matchingPizza.TotalPrice * pizzaCount;
                }
                return 0;
            });

            return Ok(totalPrice);
        }

        [HttpPost("addPizzeria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddPizzeria(PizzeriaDto pizzeria)
        {
            if (pizzeria == null)
                return BadRequest();

            if (_db.Pizzerias.Any(p => p.Name == pizzeria.Name))
                return Conflict("A pizzeria with the same name already exists.");

            _db.Pizzerias.Add(DtoToDatabase.DtoToPizzeria(pizzeria));
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPizzariaById), new { pizzariaId = pizzeria.Id }, pizzeria);
        }

        [HttpGet("pizzaria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPizzariaById(int pizzariaId)
        {
            if (pizzariaId <= 0)
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Id == pizzariaId);
            if (pizzeria == null)
                return NotFound();
            else
                return Ok(DatabaseToDto.PizzeriaToDto(pizzeria));
        }

        [HttpPut("updatePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePizzeria(string pizzeriaName, PizzeriaDto updatedPizzeria)
        {
            if (updatedPizzeria == null || string.IsNullOrEmpty(pizzeriaName))
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            if (_db.Pizzerias.Any(p => p.Id != pizzeria.Id && p.Name == updatedPizzeria.Name))
                return Conflict("A pizzeria with the same name already exists.");

            pizzeria = DtoToDatabase.DtoToPizzeria(updatedPizzeria);
            await _db.SaveChangesAsync();

            return Ok();
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

        [HttpPost("addPizzaToMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPizzaToMenu(string pizzeriaName, PizzaDto pizza)
        {
            if (pizza == null || string.IsNullOrEmpty(pizzeriaName))
                return BadRequest();

            var pizzeria = await _db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
                return NotFound();

            pizzeria.Pizzas.Add(DtoToDatabase.DtoToPizza(pizza));
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
