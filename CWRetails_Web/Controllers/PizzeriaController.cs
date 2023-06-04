using CWRetails_Web.Models;
using CWRetails_Web.Models.DTO;
using CWRetails_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CWRetails_Web.Controllers
{
    public class PizzeriaController : Controller
    {
        private readonly IPizzeriaService _pizzeriaService;

        public PizzeriaController(IPizzeriaService pizzeriaService)
        {
            _pizzeriaService = pizzeriaService;
        }

        public async Task<IActionResult> IndexPizzeria()
        {
            List<string> list = new();
            var response = await _pizzeriaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        public async Task<IActionResult> CreatePizzeria()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePizzeria(PizzeriaDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _pizzeriaService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("IndexPizzeria");
                }
            }

            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePizzeria([FromBody] PizzeriaDto model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _pizzeriaService.CreateAsync<APIResponse>(model);
        //        if (response != null && response.IsSuccess)
        //        {
        //            return RedirectToAction("IndexPizzeria");
        //        }
        //    }

        //    return View(model);
        //}


    }
}
