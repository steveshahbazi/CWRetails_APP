using AutoMapper;
using CWRetails_API.Data;
using CWRetails_API.Model;
using CWRetails_API.Model.Converter;
using CWRetails_API.Model.DTO;
using CWRetails_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CWRetails_API.Controllers
{
    [Route("api/pizzeriaAPI")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzeriaRepository _pizzeriaRepo;
        private readonly IPizzaRepository _pizzaRepo;
        private readonly IIngredientRepository _ingredientRepo;

        private readonly IMapper _mapper;
        protected APIResponse _response;

        public PizzaController(IPizzeriaRepository pizzeriaRepo, IPizzaRepository pizzaRepo, IIngredientRepository ingredientRepo, IMapper mapper)
        {
            _pizzeriaRepo = pizzeriaRepo;
            _pizzaRepo = pizzaRepo;
            _ingredientRepo = ingredientRepo;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet("pizzeriasNames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPizzeriaNames()
        {
            try
            {
                var pizzeriaNames = await _pizzeriaRepo.GetAllAsync();
                _response.Result = pizzeriaNames.Select(p => p.Name).ToList();
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpGet("pizzerias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllPizzeria()
        {
            try
            {
                var pizzerias = await _pizzeriaRepo.GetAllAsync();
                var dto = _mapper.Map<List<Pizzeria>, List<PizzeriaDto>>(pizzerias);

                _response.Result = dto;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };

                return _response;
            }
        }


        [HttpGet("menu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetMenu(string pizzeriaName)
        {
            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            var pizzas = await _pizzaRepo.GetAllAsync(p => p.PizzeriaId == pizzeria.Id);
            var menu = _mapper.Map<List<PizzaDto>>(pizzas);
            _response.Result = menu;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost("calculateTotalPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CalculateTotalPrice(OrderDto order)
        {
            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == order.PizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            if (order == null || order.Pizzas == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var menu = await _pizzaRepo.GetAllAsync(p => p.PizzeriaId == pizzeria.Id);

            bool isSubset = order.Pizzas.All(p => menu.Any(item => item.Name == p.Name));
            if (!isSubset)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            decimal totalPrice = order.Pizzas.Sum(p =>
            {
                var matchingPizza = menu.FirstOrDefault(pizza => pizza.Name == p.Name);
                if (matchingPizza != null)
                {
                    p.BasePrice = matchingPizza.BasePrice;
                    return p.TotalPrice;
                }
                return 0;
            });

            _response.Result = totalPrice;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPut("updatePizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse>> UpdatePizza(string pizzeriaName, PizzaDto updatedPizza)
        {
            if (updatedPizza == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            var pizza = await _pizzaRepo.GetAsync(p => p.PizzeriaId == pizzeria.Id && p.Id == updatedPizza.Id);
            if (pizza == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizza not found";
                return NotFound(_response);
            }

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
                    var ingredient = await _ingredientRepo.GetAsync(i => i.Id == ingredientDto.Id);

                    if (ingredient == null)
                    {
                        ingredient = new Ingredient()
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

            await _pizzaRepo.UpdateAsync(pizza);

            _response.Result = "Pizza updated successfully";
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPut("updatePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse>> UpdatePizzeria(PizzeriaDto updatedPizzeria)
        {
            if (updatedPizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Id == updatedPizzeria.Id);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            var existingPizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == updatedPizzeria.Name &&
                                                    p.Id != updatedPizzeria.Id);
            if (existingPizzeria != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria name is taken";
                return NotFound(_response);
            }

            pizzeria.Name = updatedPizzeria.Name;
            foreach (var pizza in updatedPizzeria.Pizzas)
            {
                await UpdatePizza(updatedPizzeria.Name, pizza);
            }
            
            await _pizzeriaRepo.UpdateAsync(pizzeria);

            _response.Result = "Pizzeria updated successfully";
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost("addPizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AddPizza(string pizzeriaName, PizzaDto newPizza)
        {
            if (newPizza == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            var pizza = new Pizza
            {
                Name = newPizza.Name,
                BasePrice = newPizza.BasePrice,
                Pizzeria = pizzeria,
                Ingredients = new List<Ingredient>()
            };

            foreach (var ingredientDto in newPizza.Ingredients)
            {
                var ingredient = await _ingredientRepo.GetAsync(i => i.Id == ingredientDto.Id);
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

            await _pizzaRepo.CreateAsync(pizza);

            var pizzaDto = _mapper.Map<Pizza, PizzaDto>(pizza);
            _response.Result = pizzaDto;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("getPizza")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPizza(string pizzeriaName, int pizzaId)
        {
            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            var pizza = await _pizzaRepo.GetAsync(p => p.PizzeriaId == pizzeria.Id && p.Id == pizzaId);
            if (pizza == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizza not found";
                return NotFound(_response);
            }

            var pizzaDto = _mapper.Map<Pizza, PizzaDto>(pizza);
            _response.Result = pizzaDto;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpPost("addPizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> AddPizzeria(PizzeriaDto newPizzeria)
        {
            if (newPizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var pizzeria = new Pizzeria
            {
                Name = newPizzeria.Name
            };

            await _pizzeriaRepo.CreateAsync(pizzeria);

            foreach (var newPizza in newPizzeria.Pizzas)
            {
                await AddPizza(newPizzeria.Name, newPizza);
            }

            _response.Result = "Pizzeria is added successfully!";
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{pizzeriaId}", Name = "GetPizzeriaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPizzeriaById(int pizzeriaId)
        {
            var pizzeriaEntity = await _pizzeriaRepo.GetAsync(p => p.Id == pizzeriaId);

            if (pizzeriaEntity == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            var pizzeriaDto = _mapper.Map<PizzeriaDto>(pizzeriaEntity);
            _response.Result = pizzeriaDto;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpDelete("deletePizzeria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeletePizzeria(string pizzeriaName)
        {
            var pizzeria = await _pizzeriaRepo.GetAsync(p => p.Name == pizzeriaName);
            if (pizzeria == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = "Pizzeria not found";
                return NotFound(_response);
            }

            await _pizzeriaRepo.RemoveAsync(pizzeria);

            _response.Result = "Pizzeria deleted successfully";
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
