using AutoMapper;
using CWRetails_API.Controllers;
using CWRetails_API.Data;
using CWRetails_API.Model;
using CWRetails_API.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CWRetails_Test
{
    public class MockApplicationDbContext : Mock<ApplicationDbContext>
    {
        public MockApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }


    public class PizzaControllerTests
    {
        private PizzaController _controller;
        private Mock<ApplicationDbContext> _dbMock;
        private IMapper _mapper;

        public PizzaControllerTests()
        {
            //            _dbMock = new Mock<ApplicationDbContext>();
            _dbMock = new MockApplicationDbContext(new DbContextOptions<ApplicationDbContext>());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _controller = new PizzaController(null, null, null, _mapper);
        }

        [Fact]
        public async Task GetPizzeriaNames_ReturnsOkResult()
        {
            // Arrange
            var pizzeriaNames = new List<string> { "Pizzeria 1", "Pizzeria 2", "Pizzeria 3" };
            _dbMock.Setup(db => db.Pizzerias.Select(p => p.Name).ToListAsync(default)).ReturnsAsync(pizzeriaNames);

            // Act
            var result = await _controller.GetPizzeriaNames();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsType<APIResponse>(okResult.Value);
            var response = okResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(pizzeriaNames, response.Result);
        }

        [Fact]
        public async Task GetAllPizzeria_ReturnsOkResult()
        {
            // Arrange
            var pizzerias = new List<Pizzeria> { new Pizzeria { Name = "Pizzeria 1" }, new Pizzeria { Name = "Pizzeria 2" } };
            var dto = _mapper.Map<List<Pizzeria>, List<PizzeriaDto>>(pizzerias);
            _dbMock.Setup(db => db.Pizzerias.ToListAsync(default)).ReturnsAsync(pizzerias);

            // Act
            var result = await _controller.GetAllPizzeria();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsType<APIResponse>(okResult.Value);
            var response = okResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(dto, response.Result);
        }

        [Fact]
        public async Task GetMenu_WithExistingPizzeria_ReturnsOkResult()
        {
            //// Arrange
            //var pizzeriaName = "Pizzeria 1";
            //var pizzeria = new Pizzeria { Name = pizzeriaName, Id = 1 };
            //var pizzas = new List<Pizza> { new Pizza { Name = "Pizza 1" }, new Pizza { Name = "Pizza 2" } };
            //var menu = _mapper.Map<List<Pizza>, List<PizzaDto>>(pizzas);
            //var menuQueryMock = new Mock<IQueryable<Pizza>>();
            //menuQueryMock.Setup(mq => mq.ToListAsync(default)).ReturnsAsync(pizzas);
            //_dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(It.IsAny<Expression<Func<Pizzeria, bool>>>(), default))
            //    .ReturnsAsync((Expression<Func<Pizzeria, bool>> predicate) => pizzeria);

            //_dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName, default)).ReturnsAsync(pizzeria);
            //_dbMock.Setup(db => db.Pizzas).Returns(menuQueryMock.Object);
            var pizzeriaName = "Pizzeria 1";
            var pizzeria = new Pizzeria { Name = pizzeriaName, Id = 1 };
            var pizzas = new List<Pizza> { new Pizza { Name = "Pizza 1" }, new Pizza { Name = "Pizza 2" } };
            var menu = _mapper.Map<List<Pizza>, List<PizzaDto>>(pizzas);

            var pizzaDbSetMock = new Mock<DbSet<Pizza>>();
            pizzaDbSetMock.As<IQueryable<Pizza>>().Setup(mq => mq.Provider).Returns(pizzas.AsQueryable().Provider);
            pizzaDbSetMock.As<IQueryable<Pizza>>().Setup(mq => mq.Expression).Returns(pizzas.AsQueryable().Expression);
            pizzaDbSetMock.As<IQueryable<Pizza>>().Setup(mq => mq.ElementType).Returns(pizzas.AsQueryable().ElementType);
            pizzaDbSetMock.As<IQueryable<Pizza>>().Setup(mq => mq.GetEnumerator()).Returns(pizzas.AsQueryable().GetEnumerator());

            _dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName, default)).ReturnsAsync(pizzeria);
            _dbMock.Setup(db => db.Pizzas).Returns(pizzaDbSetMock.Object);

            // Act
            var result = await _controller.GetMenu(pizzeriaName);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsType<APIResponse>(okResult.Value);
            var response = okResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(menu, response.Result);
        }

        [Fact]
        public async Task GetMenu_WithNonExistingPizzeria_ReturnsNotFoundResult()
        {
            // Arrange
            var pizzeriaName = "Non-existing Pizzeria";
            _dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName, default)).ReturnsAsync((Pizzeria)null);

            // Act
            var result = await _controller.GetMenu(pizzeriaName);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsType<APIResponse>(notFoundResult.Value);
            var response = notFoundResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CalculateTotalPrice_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var pizzeriaName = "Pizzeria 1";
            var pizzeria = new Pizzeria { Name = pizzeriaName, Id = 1 };
            var pizza1 = new Pizza { Id =1, Name = "Pizza 1", BasePrice= 10.99m };
            var pizza2 = new Pizza { Id =2, Name = "Pizza 2", BasePrice = 8.99m };
            var selectedPizzas = new List<int> { 1, 2 };

            var menuQueryMock = new Mock<IQueryable<Pizza>>();
            menuQueryMock.Setup(mq => mq.ToListAsync(default)).ReturnsAsync(new List<Pizza> { pizza1, pizza2 });
            _dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName, default)).ReturnsAsync(pizzeria);
            _dbMock.Setup(db => db.Pizzas).Returns((DbSet<Pizza>)menuQueryMock.Object);
            ToppingDto topping1 = new ToppingDto { Id = 1, Name = "Cheese", Price = 1 };
            var pizzaDto1 = new PizzaDto
            {
                Id = 1,
                Name = "Pizza 1",
                PizzaCount = 2,
                Toppings = new List<ToppingDto> { topping1 }
            };
            var pizzaDto2 = new PizzaDto { Id = 2, Name = "Pizza 2", PizzaCount = 3 };

            List<PizzaDto> pizzaDtos = new List<PizzaDto>() { pizzaDto1, pizzaDto2 };
            OrderDto orderDto = new OrderDto() { Pizzas = pizzaDtos };

            // Act
            var result = await _controller.CalculateTotalPrice(orderDto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsType<APIResponse>(okResult.Value);
            var response = okResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(19.98, response.Result);
        }

        [Fact]
        public async Task CalculateTotalPrice_WithInvalidInput_ReturnsBadRequestResult()
        {
            // Arrange
            var pizzeriaName = "Pizzeria 1";
            var pizzeria = new Pizzeria { Name = pizzeriaName, Id = 1 };
            var pizza1 = new Pizza { Id = 1, Name = "Pizza 1", BasePrice = 10.99m };
            var pizza2 = new Pizza { Id = 2, Name = "Pizza 2", BasePrice = 8.99m };
            var selectedPizzas = new List<int> { 3 }; // Non-existing pizza ID

            var menuQueryMock = new Mock<IEnumerable<Pizza>>();
            menuQueryMock.Setup(mq => mq.GetEnumerator()).Returns(new List<Pizza> { pizza1, pizza2 }.GetEnumerator());
            _dbMock.Setup(db => db.Pizzerias.FirstOrDefaultAsync(p => p.Name == pizzeriaName, default)).ReturnsAsync(pizzeria);
            _dbMock.Setup(db => db.Pizzas).Returns((DbSet<Pizza>)menuQueryMock.Object);

            ToppingDto topping1 = new ToppingDto { Id = 1, Name = "Cheese", Price = 1 };
            var pizzaDto1 = new PizzaDto
            {
                Id = 1,
                Name = "Pizza 1",
                PizzaCount = 2,
                Toppings = new List<ToppingDto> { topping1 }
            };
            var pizzaDto2 = new PizzaDto { Id = 2, Name = "Pizza 2", PizzaCount = 3 };

            List<PizzaDto> pizzaDtos = new List<PizzaDto>() { pizzaDto1, pizzaDto2 };
            OrderDto orderDto = new OrderDto() { Pizzas = pizzaDtos };

            // Act
            var result = await _controller.CalculateTotalPrice(orderDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsType<APIResponse>(badRequestResult.Value);
            var response = badRequestResult.Value as APIResponse;
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
