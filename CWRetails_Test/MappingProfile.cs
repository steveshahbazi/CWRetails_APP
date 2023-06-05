using AutoMapper;
using CWRetails_API.Model;
using CWRetails_API.Model.DTO;

namespace CWRetails_Test
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pizza, PizzaDto>().ReverseMap();
            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<Pizzeria, PizzeriaDto>().ReverseMap();
        }
    }
}