using CWRetails_Web.Models.DTO;

namespace CWRetails_Web.Controllers
{
    internal class OrderViewModel
    {
        public OrderDto Order { get; set; }
        public double TotalPrice { get; set; }
    }
}