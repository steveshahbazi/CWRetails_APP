using CWRetails_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CWRetails_Web.Services.IServices
{
    public interface IPizzeriaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> GetMenuAsync<T>(string pizzeriaName);
        Task<T> CalculateTotalPrice<T>(OrderDto order);
        Task<T> CreateAsync<T>(PizzeriaDto dto);
        Task<T> AddAsync<T>(PizzaDto dto);
        Task<T> UpdateAsync<T>(PizzeriaDto dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
