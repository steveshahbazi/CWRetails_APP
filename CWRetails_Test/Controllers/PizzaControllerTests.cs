using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CWRetails_API.Controllers;
using CWRetails_API.Model;
using CWRetails_API.Model.DTO;
using CWRetails_API.Repository.IRepository;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CWRetails_API.Tests.Controllers
{
    public class PizzaControllerTests
    {
        private readonly IPizzeriaRepository _pizzeriaRepo;
        private readonly IPizzaRepository _pizzaRepo;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IMapper _mapper;

        public PizzaControllerTests()
        {
            _pizzeriaRepo = A.Fake<IPizzeriaRepository>();
            _pizzaRepo = A.Fake<IPizzaRepository>();
            _ingredientRepo = A.Fake<IIngredientRepository>();
            _mapper = A.Fake<IMapper>();

        }

        [Fact]
        public async void PizzaController_GetPizzeriaNames_ReturnOK()
        {
            //Arrange
            var pizzeriaDtos = A.Fake<List<PizzeriaDto>>();
            var pizzerias = A.Fake<List<Pizzeria>>();
            A.CallTo(() => _mapper.Map<List<PizzeriaDto>, List<Pizzeria>>(pizzeriaDtos)).Returns(pizzerias);
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            //Act
            var result = await controller.GetPizzeriaNames();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<APIResponse>>();
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<APIResponse>(okObjectResult.Value);
            apiResponse.Result.Should().NotBeNull();
            apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PizzaController_GetPizzeriaNames_ReturnInternalServerErrorWhenExceptionThrown()
        {
            // Arrange
            A.CallTo(() => _pizzeriaRepo.GetAllAsync(null)).Throws<Exception>();
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            // Act
            var result = await controller.GetPizzeriaNames();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<APIResponse>>();
            result.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task PizzaController_CreatePizza_ReturnBadRequestWhenInvalidModel()
        {
            // Arrange
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);
            controller.ModelState.AddModelError("Name", "Name is required");
            var pizzaDto = new PizzaDto { Name = "" };

            // Act
            var result = await controller.AddPizza("test", pizzaDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<APIResponse>>();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PizzaController_CreatePizza_ReturnInternalServerErrorWhenExceptionThrown()
        {
            // Arrange
            var pizzaDto = new PizzaDto { Name = "Margherita" };
            A.CallTo(() => _pizzaRepo.CreateAsync(A<Pizza>.Ignored)).Throws<Exception>();
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            // Act
            var result = await controller.AddPizza("test", pizzaDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<APIResponse>>();
            result.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task PizzaController_AddPizza_ReturnBadRequestWhenNewPizzaIsNull()
        {
            // Arrange
            var pizzeriaName = "Test Pizzeria";
            PizzaDto newPizza = null;
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            // Act
            var result = await controller.AddPizza(pizzeriaName, newPizza);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task PizzaController_AddPizza_ReturnNotFoundWhenPizzeriaNotFound()
        {
            // Arrange
            var pizzeriaName = "Nonexistent Pizzeria";
            var newPizza = new PizzaDto
            {
                Name = "Test Pizza",
                BasePrice = 10.99m,
                Ingredients = new List<IngredientDto>()
            };
            A.CallTo(() => _pizzeriaRepo.GetAsync(A<Expression<Func<Pizzeria, bool>>>.Ignored)).Returns(Task.FromResult<Pizzeria>(null));
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            // Act
            var result = await controller.AddPizza(pizzeriaName, newPizza);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }


        [Fact]
        public async Task PizzaController_AddPizza_ReturnOkWhenPizzaAddedSuccessfully()
        {
            // Arrange
            var pizzeriaName = "Test Pizzeria";
            var newPizza = new PizzaDto
            {
                Name = "Test Pizza",
                BasePrice = 10.99m,
                Ingredients = new List<IngredientDto>()
            };
            var pizzeria = new Pizzeria { Name = pizzeriaName };
            A.CallTo(() => _pizzeriaRepo.GetAsync(A<Expression<Func<Pizzeria, bool>>>.Ignored)).Returns(pizzeria);
            A.CallTo(() => _ingredientRepo.GetAsync(A<Expression<Func<Ingredient, bool>>>._, default)).Returns((Ingredient)null);
            var controller = new PizzaController(_pizzeriaRepo, _pizzaRepo, _ingredientRepo, _mapper);

            // Act
            var result = await controller.AddPizza(pizzeriaName, newPizza);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

    }
}
