using CWRetails_Web.Models;
using CWRetails_Web.Models.DTO;
using CWRetails_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

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
            List<PizzeriaDto> list = new();
            var response = await _pizzeriaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PizzeriaDto>>(Convert.ToString(response.Result));
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

        public async Task<IActionResult> UpdatePizzeria(int termId)
        {
            var response = await _pizzeriaService.GetAsync<APIResponse>(termId);
            if (response != null && response.IsSuccess)
            {
                try
                {
                    PizzeriaDto model = JsonConvert.DeserializeObject<PizzeriaDto>(Convert.ToString(response.Result));
                    return View(model);
                }
                catch (JsonException ex)
                {
                    // Log or handle the JSON deserialization exception
                    Console.WriteLine($"JSON deserialization error: {ex.Message}");
                    return BadRequest("Invalid JSON format");
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePizzeria(PizzeriaDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _pizzeriaService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("IndexPizzeria");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> OrderTotal()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderTotal(OrderDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _pizzeriaService.CalculateTotalPrice<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    double totalPrice = (double)response.Result;
                    var orderViewModel = new OrderViewModel
                    {
                        Order = model,
                        TotalPrice = totalPrice
                    };
                    return View("OrderResult", orderViewModel);
                }
            }

            return View(model);
        }


    }
}
