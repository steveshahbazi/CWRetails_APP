using CWRetails_Web.Models;
using CWRetails_Web.Models.DTO;
using CWRetails_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CWRetails_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPizzeriaService _pizzeriaService;

        public HomeController(IPizzeriaService pizzeriaService)
        {
            _pizzeriaService = pizzeriaService;
        }

        public async Task<IActionResult> Index()
        {
            List<PizzeriaDto> list = new();
            var response = await _pizzeriaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PizzeriaDto>>(Convert.ToString(response.Result));
            }

            return View(list);
        }
    }
}