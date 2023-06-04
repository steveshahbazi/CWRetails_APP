using CWRetails_Web.Models;
using CWRetails_Web.Models.DTO;
using CWRetails_Web.Services.IServices;

namespace CWRetails_Web.Services
{
    public class PizzeriaService : BaseService, IPizzeriaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string pizzeriaUrl;

        public PizzeriaService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pizzeriaUrl = configuration.GetValue<string>("ServicesUrls:CWRetailsAPI");
        }

        public Task<T> CreateAsync<T>(PizzeriaDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Post,
                Data = dto,
                Uri = pizzeriaUrl + "/api/pizzeriaAPI/addPizzeria"
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Delete,
                Uri = pizzeriaUrl + $"/api/pizzeriaAPI/deletePizzeria"
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Get,
                Uri = pizzeriaUrl + "/api/pizzeriaAPI/pizzerias"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Get,
                Uri = pizzeriaUrl + $"/api/pizzeriaAPI/{id}"
            });
        }

        public Task<T> UpdateAsync<T>(PizzaDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Put,
                Data = dto,
                Uri = pizzeriaUrl + $"/api/pizzeriaAPI/updatePizza"
            });
        }
        public Task<T> CalculateTotalPrice<T>(OrderDto order)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Post,
                Data = order,
                Uri = pizzeriaUrl + "/api/pizzeriaAPI/calculateTotalPrice"
            });
        }

        public Task<T> GetMenuAsync<T>(string pizzeriaName)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Get,
                Uri = pizzeriaUrl + "/api/pizzeriaAPI/menu"
            });
        }

        public Task<T> AddAsync<T>(PizzaDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = CWRetails_Utility.SD.ApiType.Post,
                Data = dto,
                Uri = pizzeriaUrl + "/api/addPizza"
            });
        }
    }
}
