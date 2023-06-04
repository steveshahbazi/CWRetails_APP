using AutoMapper;
using CWRetails_API.Model;
using CWRetails_API.Model.DTO;

namespace CWRetails_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Pizza, PizzaDto>().ReverseMap();
            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<Pizzeria, PizzeriaDto>().ReverseMap();
        }
    }
}
